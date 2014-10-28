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
         public void Show(IRecipe respet)// metoden presenterar alla respeter 
    {
        Console.Clear();
        Header = respet.Name;
        ShowHeaderPanel();

        Console.WriteLine();
        Console.WriteLine("Ingredienser som behövs ");
        Console.WriteLine("-------------------------");

        foreach (Ingredient ingredient in respet.Ingredients)
        {
            Console.WriteLine(ingredient);
        }

        Console.WriteLine();
        Console.WriteLine("till laga så här");
        Console.WriteLine("-------------------------");

        int number = 0;
        foreach (var instructions in respet.Instructions)
        {
            number++;
            Console.Write(number + " ");
            Console.WriteLine(instructions);
        }
    }

         public void Show(IEnumerable<IRecipe> respet) //metoden presenterar ett recipt i taget  
    {
        foreach (Recipe recipe in respet)
        {
            Show(respet);
            ContinueOnKeyPressed();
        }
    }
}

}
