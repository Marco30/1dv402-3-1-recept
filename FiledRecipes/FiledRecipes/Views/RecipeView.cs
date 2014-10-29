using FiledRecipes.Domain;
using FiledRecipes.App.Mvp;
using FiledRecipes.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiledRecipes.Views//Marco villegas 2014-10-27
{
    public class RecipeView : ViewBase, IRecipeView
    {
        public void Show(IRecipe recepter)// metoden funktion är att strukturerar upp och presenterar recept i den svarta Console rutan 
        {
            Console.Clear();
            Header = recepter.Name;
            ShowHeaderPanel();


            Console.WriteLine("\nIngredienser som behövs ");
            Console.WriteLine("-------------------------");

            foreach (Ingredient ingredient in recepter.Ingredients)
            {
                Console.WriteLine(ingredient);
            }


            Console.WriteLine("\ntill lagas så här");
            Console.WriteLine("-------------------------");

            int number = 0;
            foreach (var instructions in recepter.Instructions)
            {
                number++;
                Console.Write("<{0}>\n", number);// visar nummren i instruktions texten    
                Console.WriteLine(instructions);
            }
        }

        public void Show(IEnumerable<IRecipe> recepter) //metoden visar alla recept 
        {
            foreach (var recept in recepter)
            {
                Show(recept);
                ContinueOnKeyPressed();
            }
        }
    }

}
