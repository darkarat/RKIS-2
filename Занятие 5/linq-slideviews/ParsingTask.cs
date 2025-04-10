using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews
{
    public enum SlideType
    {
        Theory,
        Exercise,
        Quiz
    }

    public class SlideRecord
    {
        public int SlideId { get; set; }
        public string UnitTitle { get; set; }
        public SlideType SlideType { get; set; }
    }

    public class VisitRecord
    {
        public int UserId { get; set; }
        public int SlideId { get; set; }
        public DateTime VisitTime { get; set; }
    }

    public class ParsingTask
    {
        // Метод для парсинга записей слайдов
        public static IDictionary<int, SlideRecord> ParseSlideRecords(IEnumerable<string> lines)
        {
            return lines
                .Skip(1) // Пропускаем заголовок
                .Select(line => line.Split(';'))
                .Where(parts => parts.Length == 3 &&
                                int.TryParse(parts[0], out int slideId) &&
                                !string.IsNullOrWhiteSpace(parts[1]) &&
                                Enum.TryParse<SlideType>(parts[2], true, out var slideType))
                .ToDictionary(
                    parts => int.Parse(parts[0]),
                    parts => new SlideRecord
                    {
                        SlideId = int.Parse(parts[0]),
                        UnitTitle = parts[1],
                        SlideType = slideType
                    });
        }

        // Метод для парсинга записей посещений
        public static IEnumerable<VisitRecord> ParseVisitRecords(
            IEnumerable<string> lines, IDictionary<int, SlideRecord> slides)
        {
            return lines
                .Skip(1) // Пропускаем заголовок
                .Select(line => line.Split(';'))
                .Select(parts =>
                {
                    if (parts.Length != 4 ||
                        !int.TryParse(parts[0], out int userId) ||
                        !int.TryParse(parts[1], out int slideId) ||
                        !DateTime.TryParse($"{parts[3]} {parts[2]}", out DateTime visitTime) ||
                        !slides.ContainsKey(slideId))
                    {
                        throw new FormatException("Некорректная строка: " + line);
                    }

                    return new VisitRecord
                    {
                        UserId = userId,
                        SlideId = slideId,
                        VisitTime = visitTime
                    };
                });
        }
    }
}