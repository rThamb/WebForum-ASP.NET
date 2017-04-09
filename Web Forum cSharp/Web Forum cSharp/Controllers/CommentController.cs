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
    public class CommentController : Controller
    {
        private ThreadEntities db = new ThreadEntities();


        [AdminAuthorize(Users = "admin")]
        // GET: /Comment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
           
            return View(comment);
        }

        // POST: /Comment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="CommentId,ThreadId,ParentCommentId,UserName,Date,Content")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Home", new { id = comment.ThreadId});
            }
            
            return View(comment);
        }

        [AdminAuthorize(Users = "admin")]
        // GET: /Comment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: /Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {

            //get all children of that comment 
            Comment comment = db.Comments.Find(id);

            deleteComment(comment);

            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        //will delete any sub comments 
        private Comment deleteComment(Comment comment)
        {
            if (comment != null)
            {
                var child = (from c in db.Comments
                                 where c.ParentCommentId == comment.CommentId
                                 select c).ToList<Comment>();
                //no child in 
                if (child.Count > 0)
                {
                    foreach (var com in child)
                        deleteComment(com);
                }

                db.Comments.Remove(comment);
            }

            return null; 
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
