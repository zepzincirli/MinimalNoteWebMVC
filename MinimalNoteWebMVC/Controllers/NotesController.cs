using Microsoft.AspNetCore.Mvc;
using MinimalNoteWebMVC.Models;
using System.Text;
using System.Text.Json;

namespace MinimalNoteWebMVC.Controllers
{
    public class NotesController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        public NotesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client =_httpClientFactory.CreateClient("NotesApi");
            var response = await client.GetAsync("notes");
            if(!response.IsSuccessStatusCode)
            {
                return View(new List<Note>());
            }
            var notes =await response.Content.ReadFromJsonAsync<List<Note>>();
            return View(notes);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Note note)
        {
            var client = _httpClientFactory.CreateClient("NotesApi");
            var content = new StringContent(JsonSerializer.Serialize(note),Encoding.UTF8,"application/json");
            var response= await client.PostAsync("notes",content);
        
            if(response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "An error has occurred.");
            return View(note);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var client = _httpClientFactory.CreateClient("NotesApi");
            var response = await client.GetAsync($"notes/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return View(new Note());
            }

            var note = await response.Content.ReadFromJsonAsync<Note>();
            return View(note);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("NotesApi");
            var response = await client.DeleteAsync($"notes/{id}");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("NotesApi");
            var response = await client.GetAsync($"notes/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }
            var note = await response.Content.ReadFromJsonAsync<Note>();
            return View(note);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id,Note note)
        {
            var client = _httpClientFactory.CreateClient("NotesApi");
            var content = new StringContent(JsonSerializer.Serialize(note), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"notes/{id}",content);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            ModelState.AddModelError(string.Empty, "An error has occurred.");
            return View(note);
        }
    }
}
