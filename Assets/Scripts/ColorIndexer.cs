using UnityEngine;

public class ColorIndexer : MonoBehaviour
{

    public Material blueMaterial;
    public Material greenMaterial;
    public Material greyMaterial;
    public Material whiteMaterial;


    public Material GetMat(float height){
        
        if(height <= 0.25f) {
            //color = new Color(0.12f, 0.56f, 1f);
            return blueMaterial;

        } else if(height <= 0.80f) {
            //color = new Color(0.13f, 0.55f, 0.13f);
            return greenMaterial;
        } else if(height <= 0.95f) {
            //color = new Color(0.66f, 0.66f, 0.66f);
            return greyMaterial;
        } else {
            //color = new Color(1f, 1f, 1f);
            return whiteMaterial;
        }

    }

    public Color GetColor(float height) {
        
        return GetMat(height).color;

        
         
    }
}
