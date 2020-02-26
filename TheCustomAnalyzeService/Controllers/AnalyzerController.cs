using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using TheCustomAnalyzeService.Helpers;

namespace TheCustomAnalyzeService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyzerController : ControllerBase
    {
        // POST: api/Analyzer/upload

        [HttpPost("analyze")]
        public IActionResult Analyze([FromForm] IFormFile file, [FromForm] int chartLenght)
        {
            if (file == null || chartLenght <= 0)
            {
                return Ok(new JsonErrorMessage { message = "Ошибка. Ожидается Headers: Content-Type:application/x-www-form-urlencoded; Body (form-data): file:{Путь к файлу} & chartLenght:{Количество слов в топе}" });
            }

            // Переменная для хранения текста из файла.

            var text = new StringBuilder();

            // Считывание содержимого файла в переменную text.

            using (var streamReader = new StreamReader(file.OpenReadStream()))
            {
                while (streamReader.Peek() >= 0)
                {
                    text.AppendLine(streamReader.ReadLine());
                }
            }

            // Убираю символы новой строки из полученного текста. В качестве разделителя использую знак пробела.

            var listOfWords = text.ToString().Replace(Environment.NewLine, String.Empty).Split(" ").ToList();
            listOfWords.RemoveAll(s => s == "");

            // Список, в котором отсутствуют повторения. Необходим для построения листа, который содержит информацию о частоте повторения слов.

            var filteredListOfWords = listOfWords.Distinct().ToList();

            List<WordStatistic> theStat = new List<WordStatistic>();

            /* Проверяем каждое слово из списка без повторений со всеми словами в тексте. В случае совпадения, увеличиваем аккумулятор на 1.
             В конце вложенного цикла добавляем в список сведения о слове и количестве его повторений в тексте. */

            foreach (string word in filteredListOfWords)
            {
                int countOfWordRepeats = 0;

                foreach (string anotherWord in listOfWords)
                {
                    if (anotherWord == word)
                    {
                        countOfWordRepeats++;
                    }
                }

                theStat.Add(new WordStatistic { Word = word, CountOfRepeats = countOfWordRepeats });
            }

            var result = theStat.OrderByDescending(stat => stat.CountOfRepeats).Take(chartLenght);

            return Ok(new JsonSuccessMessage<WordStatistic> { message = $"Определен набор самых повторяющихся слов в файле. Длина набора: {chartLenght}", data = result });
        }

    }
}
