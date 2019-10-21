using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using tacertoforms.Models;

namespace tacertoforms.Controllers{
    public class DisciplinasController : Controller{
        private Context db = new Context();

        // GET: Disciplinas
        public ActionResult Index(){
            List<ViewModelDisciplina> disciplinas = new List<ViewModelDisciplina>();
            foreach (var disc in db.Disciplinas.ToList()){
                ViewModelDisciplina vmDisc = new ViewModelDisciplina(){ IdDisciplina = disc.IdDisciplina, Nome = disc.Nome, Descricao = disc.Descricao };
                List<DisciplinaTurma> aux = db.DisciplinaTurmas.Where(dt => dt.IdDisciplina == disc.IdDisciplina).ToList();

                foreach (var discTurm in aux)
                    vmDisc.Turmas.Add(db.Turmas.Find(discTurm.IdTurma));
                disciplinas.Add(vmDisc);
            }
            return View(disciplinas);
        }

        //GET: Ajax
        [HttpGet]
        public ActionResult AjaxDisciplinas(int IdTurma)
        {
            List<ViewModelDisciplina> disciplinas = new List<ViewModelDisciplina>();
            List<DisciplinaTurma> disciplinasTurma = db.DisciplinaTurmas.Where(x => x.IdTurma == IdTurma).ToList();
            foreach (var disciplinaTurma in disciplinasTurma)
            {
                Disciplina disciplina = db.Disciplinas.Find(disciplinaTurma.IdDisciplina);                
                ViewModelDisciplina vmDisc = new ViewModelDisciplina() { IdDisciplinaTurma = disciplinaTurma.IdDisciplinaTurma, Nome = disciplina.Nome};
                disciplinas.Add(vmDisc);
            }
            ViewBag.DisciplinasList = new SelectList(disciplinas, "IdDisciplinaTurma", "Nome");
            return View();
        }

        // GET: Disciplinas/Details/5
        public ActionResult Details(int? id){
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Disciplina disciplina = db.Disciplinas.Find(id);
            if (disciplina == null)
                return HttpNotFound();
            return View(disciplina);
        }

        // GET: Disciplinas/Create
        public ActionResult Create(){
            ViewBag.turmas = db.Turmas.ToList();
            ViewBag.InstituicaoList = db.Instituicaos.ToList();
            return View();
        }

        // POST: Disciplinas/Create
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create(ViewModelDisciplina vmDisciplina){
            Disciplina disciplina = vmDisciplina.Disciplina;
            db.Disciplinas.Add(disciplina);
            db.SaveChanges();
            
            string[] idTurmas = vmDisciplina.idTurmas.Split(';');
            foreach (string t in idTurmas){
                db.DisciplinaTurmas.Add(new DisciplinaTurma(){ IdDisciplina = disciplina.IdDisciplina, IdTurma = int.Parse(t) });
                db.SaveChanges();
            }

            return RedirectToAction("Index");

            //return View(vmDisciplina);
        }

        // GET: Disciplinas/Edit/5
        public ActionResult Edit(int? id){
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Disciplina disciplina = db.Disciplinas.Find(id);
            if (disciplina == null)
                return HttpNotFound();

            ViewModelDisciplina viewModelDisciplina = new ViewModelDisciplina();
            viewModelDisciplina.Disciplina = disciplina;

            List<DisciplinaTurma> aux = db.DisciplinaTurmas.Where(dt => dt.IdDisciplina == viewModelDisciplina.IdDisciplina).ToList();
            foreach (var discTurm in aux)
                viewModelDisciplina.Turmas.Add(db.Turmas.Find(discTurm.IdTurma));
            viewModelDisciplina.EncherTurmas();

            ViewBag.turmas = db.Turmas.ToList();
            ViewBag.InstituicaoList = db.Instituicaos.ToList();
            
            return View(viewModelDisciplina);
        }

        // POST: Disciplinas/Edit/5
        // Para se proteger de mais ataques, ative as propriedades específicas a que você quer se conectar. Para 
        // obter mais detalhes, consulte https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Edit(ViewModelDisciplina vmDisciplina){
            Disciplina disciplina = vmDisciplina.Disciplina;
            db.Entry(disciplina).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            string[] idTurmas = vmDisciplina.idTurmas != null ? vmDisciplina.idTurmas.Split(';') : new string[0];
            foreach (var id in idTurmas)
                vmDisciplina.Turmas.Add(db.Turmas.Find(int.Parse(id)));
 
            List<DisciplinaTurma> dts = db.DisciplinaTurmas.Where(dt => dt.IdDisciplina == vmDisciplina.IdDisciplina).ToList();
            List<Turma> turmasBanco = new List<Turma>();
            foreach (var aux in dts)
                turmasBanco.Add(db.Turmas.Find(aux.IdTurma));

            for(int i = vmDisciplina.Turmas.Count - 1; i >= 0; i--){
                bool flag = false;
                for (int j = turmasBanco.Count - 1; j >= 0; j--){
                    if(turmasBanco[j].IdTurma == vmDisciplina.Turmas[i].IdTurma){
                        flag = true;
                        turmasBanco.RemoveAt(j);
                        break;
                    }
                }
                if(!flag){
                    db.DisciplinaTurmas.Add(new DisciplinaTurma() {IdDisciplina = disciplina.IdDisciplina, IdTurma = vmDisciplina.Turmas[i].IdTurma});
                    db.SaveChanges();
                }
            }
            foreach (var item in turmasBanco){
                DisciplinaTurma dt = db.DisciplinaTurmas.Where(aux => aux.IdDisciplina == vmDisciplina.IdDisciplina && aux.IdTurma == item.IdTurma).Single();
                if(dt != null){
                    db.DisciplinaTurmas.Remove(dt);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        // GET: Disciplinas/Delete/5
        public ActionResult Delete(int? id){
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Disciplina disciplina = db.Disciplinas.Find(id);
            if (disciplina == null)
                return HttpNotFound();

            ViewModelDisciplina viewModelDisciplina = new ViewModelDisciplina();
            viewModelDisciplina.Disciplina = disciplina;

            List<DisciplinaTurma> aux = db.DisciplinaTurmas.Where(dt => dt.IdDisciplina == viewModelDisciplina.IdDisciplina).ToList();
            foreach (var discTurm in aux)
                viewModelDisciplina.Turmas.Add(db.Turmas.Find(discTurm.IdTurma));
            viewModelDisciplina.EncherTurmas();

            ViewBag.turmas = db.Turmas.ToList();
            ViewBag.InstituicaoList = db.Instituicaos.ToList();
            
            return View(viewModelDisciplina);
        }

        // POST: Disciplinas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id){
            Disciplina disciplina = db.Disciplinas.Find(id);
            db.Disciplinas.Remove(disciplina);
            db.SaveChanges();
            List<DisciplinaTurma> disciplinaTurmas = db.DisciplinaTurmas.Where(dt => dt.IdDisciplina == id).ToList();
            foreach (var dt in disciplinaTurmas){
                db.DisciplinaTurmas.Remove(dt);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing){
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}