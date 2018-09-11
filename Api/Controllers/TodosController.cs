using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Api.Controllers
{
    public class Todo
    {
        public Guid Id { get; set; }
        [Required] public string Title { get; set; }
        public bool Completed { get; set; } = false;
    }

    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class TodosController : ControllerBase
    {
        IDocumentStore store ;
        public TodosController(IDocumentStore store) => this.store = store;
        
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Todo>> Get()
        {
            int count;
            List<Todo> todos;

            using (var sess = store.OpenSession())
            {
                count = sess.Query<Todo>().Count();
                var ucount = sess.Query<User>().Count();
            }

            if (count == 0)
            {
                using (var session = store.LightweightSession())
                {
                    session.Store(new Todo {Title = "first one"});
                    session.Store(new Todo {Title = "second one"});
                    session.SaveChanges();
                }
            }

            using (var sess = store.LightweightSession())
            {
                todos = sess.Query<Todo>().ToList();
            }

            return Ok(todos);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<Todo> Get(string id)
        {
            using (var sess = store.LightweightSession())
            {
                return Ok( sess.Load<Todo>(Guid.Parse(id)));
            }
        }

        // POST api/values
        [HttpPost]
        public ActionResult<Todo> Post([FromBody] Todo todo)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            using (var sess = store.OpenSession())
            {
                sess.Store(todo);
                sess.SaveChanges();
   
                return Created(nameof(Get), todo);
            }

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}