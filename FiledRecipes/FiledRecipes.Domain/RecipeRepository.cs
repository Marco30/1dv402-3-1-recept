using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FiledRecipes.Domain //Marco villegas 2014-10-27
{
    /// <summary>
    /// Holder for recipes.
    /// </summary>
    public class RecipeRepository : IRecipeRepository
    {
        /// <summary>
        /// Represents the recipe section.
        /// </summary>
        private const string SectionRecipe = "[Recept]";
        /// <summary>
        /// Represents the ingredients section.
        /// </summary>
        private const string SectionIngredients = "[Ingredienser]";
        /// <summary>
        /// Represents the instructions section.
        /// </summary>
        private const string SectionInstructions = "[Instruktioner]";
        /// <summary>
        /// Occurs after changes to the underlying collection of recipes.
        /// </summary>
        public event EventHandler RecipesChangedEvent;
        /// <summary>
        /// Specifies how the next line read from the file will be interpreted.
        /// </summary>
        private enum RecipeReadStatus { Indefinite, New, Ingredient, Instruction };
        /// <summary>
        /// Collection of recipes.
        /// </summary>
        private List<IRecipe> _recipes;
        /// <summary>
        /// The fully qualified path and name of the file with recipes.
        /// </summary>
        private string _path;
        /// <summary>
        /// Indicates whether the collection of recipes has been modified since it was last saved.
        /// </summary>
        public bool IsModified { get; protected set; }
        /// <summary>
        /// Initializes a new instance of the RecipeRepository class.
        /// </summary>
        /// <param name="path">The path and name of the file with recipes.</param>
        public RecipeRepository(string path)
        {
            // Throws an exception if the path is invalid.
            _path = Path.GetFullPath(path);
            _recipes = new List<IRecipe>();
        }
        /// <summary>
        /// Returns a collection of recipes.
        /// </summary>
        /// <returns>A IEnumerable&lt;Recipe&gt; containing all the recipes.</returns>
        public virtual IEnumerable<IRecipe> GetAll()
        {
            // Deep copy the objects to avoid privacy leaks.
            return _recipes.Select(r => (IRecipe)r.Clone());
        }
        /// <summary>
        /// Returns a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to get.</param>
        /// <returns>The recipe at the specified index.</returns>
        public virtual IRecipe GetAt(int index)
        {
            // Deep copy the object to avoid privacy leak.
            return (IRecipe)_recipes[index].Clone();
        }
        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="recipe">The recipe to delete. The value can be null.</param>
        public virtual void Delete(IRecipe recipe)
        {
            // If it's a copy of a recipe...
            if (!_recipes.Contains(recipe))
            {
                // ...try to find the original!
                recipe = _recipes.Find(r => r.Equals(recipe));
            }
            _recipes.Remove(recipe);
            IsModified = true;
            OnRecipesChanged(EventArgs.Empty);
        }
        /// <summary>
        /// Deletes a recipe.
        /// </summary>
        /// <param name="index">The zero-based index of the recipe to delete.</param>
        public virtual void Delete(int index)
        {
            Delete(_recipes[index]);
        }
        /// <summary>
        /// Raises the RecipesChanged event.
        /// </summary>
        /// <param name="e">The EventArgs that contains the event data.</param>
        protected virtual void OnRecipesChanged(EventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler handler = RecipesChangedEvent;
            // Event will be null if there are no subscribers.
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }
        public void Load()// metoden kommer lada in recept somfinns i mapen AppData/ recipes.txt
        {
            RecipeReadStatus receptStatus = new RecipeReadStatus();
            Recipe receptObject = null;
            List<IRecipe> recept = new List<IRecipe>();//deklarerar dynamic arrays
            string line;

            using (StreamReader reader = new StreamReader(_path))
            {


                while ((line = reader.ReadLine()) != null)//inuti den här den här while satsen har vi funktionerna som läser in texten som finns i Recipes.txt
                {

                    //if satserna bestämmer raden status i texten med andra ord vad det är, om det är titel, ingrediens eller instruktion på hur man lagar maten 
                    if (line == "[Recept]")
                    {
                        receptStatus = RecipeReadStatus.New;
                    }
                    else if (line == "[Ingredienser]")
                    {
                        receptStatus = RecipeReadStatus.Ingredient;
                    }
                    else if (line == "[Instruktioner]")
                    {
                        receptStatus = RecipeReadStatus.Instruction;
                    }


                    else
                    {

                        switch (receptStatus) // bestämmer hur man hanterar de olika raderna beroende på vilket status den fåt   
                        {



                            case RecipeReadStatus.New:// om det är en tittel så tas det han om här
                                receptObject = new Recipe(line);
                                recept.Add(receptObject);
                                break;


                            case RecipeReadStatus.Ingredient: // om det är en Ingredienser så tas det hand om här 

                                string[] ingredienser = line.Split(';');


                                if (ingredienser.Length != 3)
                                {
                                    throw new FileFormatException();
                                }

                                Ingredient ingredient = new Ingredient(); // skapa ingredientobjekt 

                                ingredient.Amount = ingredienser[0]; // läger in mängd i ingredientobjekt, 
                                ingredient.Measure = ingredienser[1];// läger in mått i ingredientobjek
                                ingredient.Name = ingredienser[2];// läger in namn i ingredientobjek


                                receptObject.Add(ingredient);// läger till ingredient till recipe-objectet
                                break;

                            case RecipeReadStatus.Instruction:// om det är en Instruktioner så tas det hand om här 
                                if (line.Length > 0)
                                {
                                    receptObject.Add(line);// läger till Instruction till recipe-objectet
                                }
                                break;

                            default:
                                throw new FileFormatException();
                        }
                    }

                    recept.Sort();// sorterar lista 
                    _recipes = recept;// läger till sorterad lista till _recipes
                    IsModified = false;
                    OnRecipesChanged(EventArgs.Empty);
                }

            }

        }


        public void Save()// metoden kommer att spara ändrignar som gjort i recipes.txt somfinns i mapen AppData/ recipes.txt
        {


        }

    }
}