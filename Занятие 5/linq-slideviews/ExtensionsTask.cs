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

    public class VisitRecord
    {
        public int UserId { get; set; }
        public int SlideId { get; set; }
        public TimeSpan Time { get; set; }
        public DateTime Date { get; set; }
    }

    public class SlideRecord
    {
        public int SlideId { get; set; }
        public string UnitTitle { get; set; }
        public SlideType SlideType { get; set; }
    }

    public static class StatisticsTask
    {
        public static double GetMedianTimePerSlide(IEnumerable<VisitRecord> visits, IDictionary<int, SlideRecord> slides, SlideType slideType)
        {
            // Фильтруем записи посещений по типу слайда
            var filteredVisits = visits
                .Where(v => slides.TryGetValue(v.SlideId, out var slide) && slide.SlideType == slideType)
                .GroupBy(v => v.SlideId)
                .Select(g => g.Select(v => v.VisitTime).ToList())
                .Where(times => times.Count > 1) // Убираем группы с менее чем двумя записями
                .Select(times =>
                {
                    // Вычисляем время между первым и последним посещением
                    var totalMinutes = (times.Max() - times.Min()).TotalMinutes;
                    return totalMinutes / times.Count; // Среднее время на слайд
                })
                .OrderBy(t => t)
                .ToList();

            // Вычисляем медиану
            if (filteredVisits.Count == 0) return 0;

            int midIndex = filteredVisits.Count / 2;
            return filteredVisits.Count % 2 == 0 ? (filteredVisits[midIndex - 1] + filteredVisits[midIndex]) / 2 : filteredVisits[midIndex];
        }
    }

    public static class ExtensionsTask
    {
        // Метод для вычисления медианы из последовательности чисел
        public static double GetMedian(this IEnumerable<double> items)
        {
            if (items == null || !items.Any())
            {
                throw new InvalidOperationException("Последовательность не должна быть пустой.");
            }

            var sortedItems = items.OrderBy(x => x).ToList();
            int count = sortedItems.Count;

            if (count % 2 == 1) // Нечетное количество элементов
            {
                return sortedItems[count / 2];
            }
            else // Четное количество элементов
            {
                return (sortedItems[(count / 2) - 1] + sortedItems[count / 2]) / 2.0;
            }
        }

        // Метод для получения биграмм из последовательности элементов
        public static IEnumerable<(T First, T Second)> GetBigrams<T>(this IEnumerable<T> items)
        {
            using (var enumerator = items.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    yield break; // Если нет элементов, выходим
                }

                T previous = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    T current = enumerator.Current;
                    yield return (previous, current);
                    previous = current;
                }
            }
        }
    }
}