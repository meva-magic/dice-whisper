using System.Collections.Generic;

[System.Serializable]
public class Dialog
{
    public string Name;
    public List<DialogItem> Items;
}

[System.Serializable]
public class DialogItem
{
    public string CharacterName;
    public string Text;
    public bool StartsDiceGame = false;
}
