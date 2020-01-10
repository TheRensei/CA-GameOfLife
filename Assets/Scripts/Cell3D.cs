using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell3D : MonoBehaviour
{
    public int state = 0;
    public int[] position;
    MeshRenderer meshRend;

    ///<summary>
    /// Creates the cell
    ///</summary>
    /// <param name="pos">Position of the cell</param>
    /// <param name="newMat">Material to be used by the cell</param>
    public void CreateCell(int[] pos, Material newMat)
    {
        state = 0;
        position = pos;
        this.transform.position = new Vector3(pos[0], pos[1], pos[2]);
        meshRend = this.gameObject.GetComponent<MeshRenderer>();
        SetState(state);
        SetMaterial(newMat);
    }

    ///<summary>
    /// Set the cells state and the rule that was used on it
    ///</summary>
    /// <param name="newStatus">New status of the cell</param>
    public void SetState(int newStatus)
    {
        state = newStatus;
        meshRend.enabled = state == 1 ? true : false;
        meshRend.material.color = new Color(state, state, state, 0.1f);
    }


    ///<summary>
    /// Set the material to be used by the cell and initial colour
    ///</summary>
    /// <param name="m">New Material to be used</param>
    void SetMaterial(Material m)
    {
        meshRend.material = m;
        meshRend.material.color = new Color(state, state, state, 0.5f);
    }
}
