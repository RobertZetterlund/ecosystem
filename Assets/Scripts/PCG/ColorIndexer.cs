using UnityEngine;

public class ColorIndexer : MonoBehaviour
{

    public Gradient blueMaterial;
    public Gradient greenMaterial;
    public Gradient greyMaterial;
    public Gradient whiteMaterial;




    public Color GetColor(float height) {
        
        

        if(height <= 0.25f) {
            return blueMaterial.Evaluate(4*height);

        } else if(height <= 0.80f) {
            return greenMaterial.Evaluate(1f/0.55f * (height-0.25f));
        } else if(height <= 0.95f) {

            return greyMaterial.Evaluate(1f/0.15f * (height - 0.85f));
        } else {

            return whiteMaterial.Evaluate(1f/0.05f * (height - 0.95f));
        }

    }
}
