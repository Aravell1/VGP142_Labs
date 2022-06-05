[System.Serializable]
public class PlayerData
{
    public float posX;
    public float posY;
    public float posZ;

    public float rotX;
    public float rotY;
    public float rotZ;

    public bool playerInfoStored = false;

    public string curScene = "\0";

    public string CurrentDate = "\0";
    public int lives;
    public int score;
}
