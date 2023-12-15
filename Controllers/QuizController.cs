
using lab13.Interfaces;
using lab13.Models;
using Microsoft.AspNetCore.Mvc;

namespace lab13.Controllers
{
    public class QuizController : Controller
    {
        private readonly IQuizService _quizServ;
        private readonly IQuestionsStorageService _questionsStorageService;

        public QuizController(IQuizService quizService, IQuestionsStorageService questionsStorageService)
        {
            _quizServ = quizService;
            _questionsStorageService = questionsStorageService;
        }

        public IActionResult Index()
        {
            _questionsStorageService.Clear();

            return View();
        }

        public IActionResult QuestionForm()
        {
            QuestionViewModel questionViewModel = _quizServ.CreateQuestion();

            var data = _questionsStorageService.GetAll();

            return View(questionViewModel);
        }

        [HttpPost]
        public IActionResult QuestionProcess(QuestionViewModel questionViewModel)
        {
            int RightAnswer = _quizServ.GetRightAnswer(questionViewModel);
            bool answerIsValid = _quizServ.CheckAnswer(questionViewModel);
            
            questionViewModel.AnswerIsValid = answerIsValid;
            questionViewModel.RightAnswer = RightAnswer;

            _questionsStorageService.Add(questionViewModel);

            if (questionViewModel.BtnPressed == "next")
                return RedirectToAction("QuestionForm");
            
            return RedirectToAction("ViewResults");
        }

        public IActionResult ViewResults()
        {

            var questions = _questionsStorageService.GetAll();
            int rightAnswersAmount = _quizServ.GetRightAnswersAmount(questions);
            int questionsAmount = questions.Count();

            ResultsViewModel resultsViewModel = new ResultsViewModel
            {
                RightAnswersAmount = rightAnswersAmount,
                QuestionsAmount = questionsAmount,
                Questions = questions
            };

            return View(resultsViewModel);
        }
    }
}
