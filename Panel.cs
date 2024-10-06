using System;
using System.Collections;

namespace BreadEngine; 

public enum PanelNavigationAction
{
    Stay,
    NextPanel,
    PreviousPanel
}

public class Panel 
{
    public ArrayList cells = new ArrayList();
    public ArrayList components = new ArrayList();
    public ArrayList interactableComponents = new ArrayList();
    private ArrayList cellsVerticalMatrix = new ArrayList();
    public ConsoleColor borderColor = ConsoleColor.White;

    public int selectedIndex = -1;

    private int panelCount;
    private bool hasTitle = false;
    private string titlestring = "";
    private Component title = null;
    private int titleIndex = 0;
    public Panel(ArrayList components, int panelCount){
        this.components = components;
        this.panelCount = panelCount;

        //To prevent being empty and throwing errors
        if(components.Count == 0)
        {
            components.Add(new Spacer());
        }

        

        //Handling of the title and removing it
        //from the regular components arraylist
        foreach(Component component in components) 
        {
            if(typeof(Title) == component.GetType()) 
            {
                hasTitle = true;
                title = component;
                components.Remove(component);
                break;
            }
        }

        //Has to come after removing title (+1h dev time)
        for(int i = 0; i < components.Count; i++) 
        {
            if(((Component) components[i]).interactable) 
            {
                interactableComponents.Add(i);
            }
        }
    }

    public Component GetComponent(string uid)
    {
        foreach (Component component in components) 
        {
            if (component.uid == uid) 
            {
                return component;
            }
        }
        return null;
    }

    bool firstDrawCall = true;
    public void Draw()
    {
        if(firstDrawCall) 
        {
            //Fill the cellsverticalmatrix with empty arrays
            for(int i = 0; i < /*Count of rows, need outsideinfo*/panelCount; i++) 
            {
                cellsVerticalMatrix.Add(new ArrayList());
            }
            //Add cells to cellsverticalmatrix based on their
            //matrixY position
            for(int i = 0; i < cells.Count; i++) 
            {
                Cell cell = (Cell) cells[i];
                ((ArrayList) cellsVerticalMatrix[cell.matrixY]).Add(cell);
            }
            //remove the elements from the arraylist
            //with the help of another arrylist
            ArrayList newArray = new ArrayList();
            for(int i = 0; i < cellsVerticalMatrix.Count; i++) 
            {
                if(((ArrayList) cellsVerticalMatrix[i]).Count != 0) 
                {
                    newArray.Add(cellsVerticalMatrix[i]);
                }
            }
            cellsVerticalMatrix = newArray;

            firstDrawCall = false;
        }

        int ScreenWidth = FastConsole.Width;
        int ScreenHeight = FastConsole.Height;

        


        int objectIndex = 0;
        int textIndex = 0;
        for (int rowCount = 0; rowCount < cellsVerticalMatrix.Count; rowCount++) {
            ArrayList currentRow = (ArrayList)cellsVerticalMatrix[rowCount];
            Cell lastCell = (Cell)currentRow[currentRow.Count-1];
            for (int y = 0; y < ((Cell)currentRow[0]).Height; y++) {
                for (int cellIndex = 0; cellIndex < currentRow.Count; cellIndex++) {
                    Cell cell = (Cell)currentRow[cellIndex];
                    bool l,r,t,b;
                    l = isOurCell(cell.matrixX-1, cell.matrixY);
                    r = isOurCell(cell.matrixX+1, cell.matrixY);
                    t = isOurCell(cell.matrixX, cell.matrixY-1);
                    b = isOurCell(cell.matrixX, cell.matrixY+1);
                    for (int x = 0; x < cell.Width; x++) {
                        char toWrite = '*';
                        ConsoleColor foreColor = borderColor;
                        ConsoleColor backColor = ConsoleColor.Black;

                        //TODO customizable borders
                        if (!t && !l && x == 0 && y == 0) {
                            toWrite = '┌';
                        } else if(!t && !r && x == cell.Width-1 && y == 0) {
                            toWrite = '┐';
                        } else if(!l && !b && x == 0 && y == cell.Height-1) {
                            toWrite = '└';
                        } else if(!r && !b && x == cell.Width-1 && y == cell.Height-1) {
                            toWrite = '┘';
                        } else if((x == 0 && !l) || (x == cell.Width-1 && !r)) {
                            toWrite = '│';
                        } else if(y == 0 && !t) {
                            if(hasTitle){
                                //Drawing titles
                                if(x == 1) {
                                    //First char of title
                                    titlestring = new String(title.Draw(cell.Width * currentRow.Count - 2));
                                }
                                if(titleIndex >= titlestring.Length){
                                    toWrite = '─';
                                }else{
                                    toWrite = titlestring[titleIndex];
                                    titleIndex++;
                                }
                            }else{
                                toWrite = '─';
                            }
                        } else if(y == cell.Height-1 && !b) {
                            toWrite = '─';
                        } else {
                            // FastConsole.ResetColor();
                            if(objectIndex >= components.Count) {
                                toWrite = ' ';
                            } else {
                                Component component = (Component)components[objectIndex];
                                string objString = new(component.Draw(cell.Width * currentRow.Count - 2));
                                foreColor = component.foreground;
                                if(selectedIndex == objectIndex){
                                    backColor = ConsoleColor.Red;
                                }
                                
                                #region Drawing of string onto the screen
                                if(textIndex >= objString.Length){
                                    //If textIndex out of range
                                    if(cell.Equals(lastCell) && x == cell.Width-2) {
                                        //Last char of line
                                        if (textIndex < objString.Length) {
                                            //Haven't finished string
                                        } else {
                                            //Finished string
                                            textIndex = 0;
                                            objectIndex++;
                                        }
                                        toWrite = ' ';
                                    } else {
                                        toWrite = ' ';
                                    }
                                } else {
                                    //Textindex is not out of range
                                    if(textIndex == objString.Length-1 && cell.Equals(lastCell) && x == cell.Width-2) {
                                            toWrite = objString[textIndex];
                                            textIndex = 0;
                                            objectIndex++;
                                    } else {
                                        toWrite = objString[textIndex];
                                        textIndex++;
                                    }
                                }
                                #endregion
                            }
                        }
                        
                        FastConsole.SetCursor((cell.Width * cell.matrixX) + x, (cell.Height * cell.matrixY) + y);
                        FastConsole.Write(toWrite, foreColor, backColor);
                    }
                }
            }
        }
    }

    public PanelNavigationAction OnKey(ConsoleKeyInfo keyInfo){
        switch (((Component)components[selectedIndex]).OnKey(keyInfo)) {
            case ComponentNavigationAction.Stay:
                //Do nothing
                break;
            case ComponentNavigationAction.NextComponent:
                int i = interactableComponents.IndexOf(selectedIndex);
                if(i == interactableComponents.Count - 1) {
                    //Run out of components
                    selectedIndex = -1;
                    return PanelNavigationAction.NextPanel;
                } else {
                    //Go to next component
                    selectedIndex = (int)interactableComponents[i+1];
                }
                break;
            case ComponentNavigationAction.NextPanel:
                selectedIndex = -1;
                return PanelNavigationAction.NextPanel;
            case ComponentNavigationAction.PreviousComponent:
                int x = interactableComponents.IndexOf(selectedIndex);
                if(x == 0) {
                    //Out of bounds
                    selectedIndex = -1;
                    return PanelNavigationAction.PreviousPanel;
                } else {
                    //Previous component
                    selectedIndex = (int)interactableComponents[x - 1];
                }
                break;
            case ComponentNavigationAction.PreviousPanel:
                selectedIndex = -1;
                return PanelNavigationAction.PreviousPanel;
            default:
                //Do nothing
                break;
        }
        return PanelNavigationAction.Stay;
    }

    // This function returns weather we own a
    // cell given it's coordinates in the matrix
    public bool isOurCell(int x, int y) {
        foreach (Cell cell in cells) {
            if(cell.matrixX == x && cell.matrixY == y) {
                return true;
            }
        }
        return false;
    }
}

public struct Cell{
    public int Width, Height, matrixX, matrixY;

    public override bool Equals(object obj) {
        if(obj.GetType() == this.GetType()) {
            Cell other = (Cell)obj;
            return (matrixX == other.matrixX) && (matrixY == other.matrixY);
        } else {
            return false;
        }
    }

    //Put this here so that the warning goes away
    public override int GetHashCode() {
        return HashCode.Combine(Width, Height, matrixX, matrixY);
    }
}