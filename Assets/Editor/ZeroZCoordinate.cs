using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ZeroZCoordinate
{
    [MenuItem( "Tools/ZeroObjectZ #z" )]
    private static void ZeroObjectZ()
    {
        if ( Selection.activeGameObject != null )
            Selection.activeGameObject.transform.Translate( new Vector3( 0, 0, -Selection.activeGameObject.transform.position.z ), Space.World );
    }
}
