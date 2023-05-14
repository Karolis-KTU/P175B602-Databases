﻿namespace KompiuteriuPardavimas.Controllers;

using KompiuteriuPardavimas.Models;
using KompiuteriuPardavimas.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


/// <summary>
/// Controller for working with 'Darbuotojas' entity.
/// </summary>
public class DarbuotojasController : Controller
{
	/// <summary>
	/// This is invoked when either 'Index' action is requested or no action is provided.
	/// </summary>
	/// <returns>Entity list view.</returns>
	[HttpGet]
	public ActionResult Index()
	{
		//gražinamas darbuotoju sarašo vaizdas
		return View(DarbuotojasRepository.List());
	}

	/// <summary>
	/// This is invoked when creation form is first opened in browser.
	/// </summary>
	/// <returns>Creation form view.</returns>
	[HttpGet]
	public ActionResult Create()
	{
		var darb = new DarbuotojasCE();
		PopulateSelections(darb);
		return View(darb);
	}

	/// <summary>
	/// This is invoked when buttons are pressed in the creation form.
	/// </summary>
	/// <param name="darb">Entity model filled with latest data.</param>
	/// <returns>Returns creation from view or redirects back to Index if save is successfull.</returns>
	[HttpPost]
	public ActionResult Create(DarbuotojasCE darb)
	{
		//do not allow creation of entity with 'tabelis' field matching existing one
		var match = DarbuotojasRepository.Find(darb.Darbuotojas.Kodas);

		if( match !=null )
			ModelState.AddModelError("Kodas", "Field value already exists in database.");

		//form field validation passed?
		if (ModelState.IsValid)
		{
			//NOTE: insert will fail if someone managed to add different 'darbuotojas' with same 'tabelis' after the check
			DarbuotojasRepository.Insert(darb);

			//save success, go back to the entity list
			return RedirectToAction("Index");
		}

		//form field validation failed, go back to the form
		Console.WriteLine("Here");
		return View(darb);

	}

	/// <summary>
	/// This is invoked when editing form is first opened in browser.
	/// </summary>
	/// <param name="id">ID of the entity to edit.</param>
	/// <returns>Editing form view.</returns>
	[HttpGet]
	public ActionResult Edit(int id)
	{
		return View(DarbuotojasRepository.Find(id));
	}

	/// <summary>
	/// This is invoked when buttons are pressed in the editing form.
	/// </summary>
	/// <param name="id">ID of the entity being edited</param>		
	/// <param name="darb">Entity model filled with latest data.</param>
	/// <returns>Returns editing from view or redirects back to Index if save is successfull.</returns>
	[HttpPost]
	public ActionResult Edit(string id, DarbuotojasCE darb)
	{
		//form field validation passed?
		if (ModelState.IsValid)
		{
			DarbuotojasRepository.Update(darb);

			//save success, go back to the entity list
			return RedirectToAction("Index");
		}

		//form field validation failed, go back to the form
		return View(darb);
	}

	/// </summary>
	/// <param name="id">ID of the entity to delete.</param>
	/// <returns>Deletion form view.</returns>
	[HttpGet]
	public ActionResult Delete(int id)
	{
		var darb = DarbuotojasRepository.Find(id);
		return View(darb);
	}

	/// <summary>
	/// This is invoked when deletion is confirmed in deletion form
	/// </summary>
	/// <param name="id">ID of the entity to delete.</param>
	/// <returns>Deletion form view on error, redirects to Index on success.</returns>
	[HttpPost]
	public ActionResult DeleteConfirm(int id)
	{
		//try deleting, this will fail if foreign key constraint fails
		try 
		{
			DarbuotojasRepository.Delete(id);

			//deletion success, redired to list form
			return RedirectToAction("Index");
		}
		//entity in use, deletion not permitted
		catch( MySql.Data.MySqlClient.MySqlException )
		{
			//enable explanatory message and show delete form
			ViewData["deletionNotPermitted"] = true;

			var darb = DarbuotojasRepository.Find(id);
			return View("Delete", darb);
		}
	}

	public void PopulateSelections(DarbuotojasCE darbuotojasCE)
	{
		// load entities for the select lists
		var biurai = DarbuotojasRepository.ListBiurai();

		// build select lists
		darbuotojasCE.Lists.Biurai =
			biurai.Select(it =>
			{
				return
					new SelectListItem()
					{
						Value = Convert.ToString(it.ID),
						Text = it.Pavadinimas
					};
			})
			.ToList();
	}
}
