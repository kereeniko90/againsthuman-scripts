using UnityEngine;
using System.Collections.Generic;

public class Level_01 : MonoBehaviour
{
    [SerializeField]
    private List<Waypoint> path = new List<Waypoint>();

    public List<Waypoint> WaypointList() {
        return path;
    }
}
