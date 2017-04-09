using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_Forum_cSharp.Models;
using Web_Forum_cSharp.Validators;


namespace Web_Forum_cSharp.Controllers
{
    public class HomeController : Controller
    {
        private ThreadEntities db = new ThreadEntities();

        // GET: Home
        public ActionResult Index()
        {          
            var threadToShow = (from t in db.Threads.Include(t => t.UserDetail)
                                orderby t.Views descending
                                select t).Take(20).ToList();

            return View(threadToShow);
        }




        // GET: Home/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thread thread = db.Threads.Find(id);
            if (thread == null)
            {
                return HttpNotFound();
            }

            // increment the view and save the change          
            thread.Views += 1; 
            db.SaveChanges();

            //get all the top level comment 

            var comment = (from c in db.Comments
                           where c.ParentCommentId.Equals(null) && c.ThreadId == (int)id
                           select c).ToList<Comment>();

            ViewBag.TopLevelComments = comment; 
            
            return View(thread);
        }

        //Must be logged in to like a thread 
        [Authorize]
        public ActionResult upVoteThread(int? id)
        {
            if(id == null || id == 0) 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            
            Thread thread = db.Threads.Find(id);

            if (thread != null)
            {
                thread.Upvotes += 1;
                db.SaveChanges();
            }

            return RedirectToAction("Index"); 
        }



        [Authorize]
        //GET 
        public ActionResult ReplyComment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Comment parentCommment = db.Comments.Find(id); 

            int threadID = parentCommment.ThreadId;

            Comment comment = new Comment();

            comment.ThreadId = threadID;
            comment.ParentCommentId = parentCommment.CommentId; 
            comment.UserName = User.Identity.Name;
            comment.Date = System.DateTime.Now;
           
            return View(comment);
        }


        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReplyComment([Bind(Include = "ThreadId,ParentCommentId,UserName,Date,Content")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "Home", new { id = comment.ThreadId });
            }

            return View(comment);
        }

        //reply to Thread 

        [Authorize]
        public ActionResult ReplyThread(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            int threadID = (int)id;

            Comment comment = new Comment();

            comment.ThreadId = threadID;
            comment.ParentCommentId = null;
            comment.UserName = User.Identity.Name;
            comment.Date = System.DateTime.Now;

            return View(comment);


        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReplyThread([Bind(Include = "CommentId,ThreadId,ParentCommentId,UserName,Date,Content")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Add(comment);
                db.SaveChanges();
                return RedirectToAction("Details", "Home", new { id = comment.ThreadId });
            }

            return View(comment);
        }


        [Authorize]
        // GET: Home/Create
        public ActionResult Create()
        {
            String username = User.Identity.Name;
            // the Identity is not null when u being

            if (username == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //creat a thread object with starting values, thread primary is auto 

            Thread newThread = new Thread();
            newThread.Date = DateTime.Now;
            newThread.Upvotes = 0;
            newThread.Views = 0;
            newThread.UserName = username; 

            //get all the tags 

            var AllTags = (from t in db.Tags
                               select t).ToList<Tag>();

            ViewBag.TagsForThread = AllTags; 
           

            return View(newThread);
        }

        // POST: Home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ThreadId,Title,Content,UserName,Date,Views,Upvotes")] Thread thread, IEnumerable<int> tagsPicked)
        {
            if (ModelState.IsValid)
            {

                if (tagsPicked != null)
                {
                    //insert to Tagthread table 
                    foreach (int tagid in tagsPicked)
                    {
                        TagThread link = new TagThread();
                        link.ThreadId = thread.ThreadId;
                        link.TagId = tagid;

                        thread.TagThreads.Add(link);
                    }
                }

                db.Threads.Add(thread);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(thread);
        }


        [AdminAuthorize(Users = "admin")]
        // GET: Home/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thread thread = db.Threads.Find(id);
            if (thread == null)
            {
                return HttpNotFound();
            }

            return View(thread);
        }

        // POST: Home/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ThreadId,Title,Content,UserName,Date,Views,Upvotes")] Thread thread)
        {
            if (ModelState.IsValid)
            {
                db.Entry(thread).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(thread);
        }

        [AdminAuthorize(Users = "admin")]
        // GET: Home/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Thread thread = db.Threads.Find(id);
            if (thread == null)
            {
                return HttpNotFound();
            }
            return View(thread);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Thread thread = db.Threads.Find(id);

            db.Threads.Remove(thread);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
