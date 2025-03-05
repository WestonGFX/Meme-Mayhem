    private void PlaceSpecialRooms()
    {
        List<Vector2Int> normalRooms = new List<Vector2Int>();
        
        // Find all normal rooms
        foreach (var roomEntry in roomMap)
        {
            if (roomEntry.Value == RoomGenerator.RoomType.Normal)
            {
                normalRooms.Add(roomEntry.Key);
            }
        }
        
        // Make sure we have enough rooms to place special ones
        if (normalRooms.Count < 4)
            return;
            
        // Shuffle the list
        for (int i = 0; i < normalRooms.Count; i++)
        {
            int randomIndex = Random.Range(i, normalRooms.Count);
            Vector2Int temp = normalRooms[i];
            normalRooms[i] = normalRooms[randomIndex];
            normalRooms[randomIndex] = temp;
        }
        
        // Place item room
        if (Random.value <= itemRoomChance && normalRooms.Count > 0)
        {
            Vector2Int itemRoomPos = normalRooms[0];
            roomMap[itemRoomPos] = RoomGenerator.RoomType.Item;
            normalRooms.RemoveAt(0);
        }
        
        // Place shop room
        if (Random.value <= shopRoomChance && normalRooms.Count > 0)
        {
            Vector2Int shopRoomPos = normalRooms[0];
            roomMap[shopRoomPos] = RoomGenerator.RoomType.Shop;
            normalRooms.RemoveAt(0);
        }
        
        // Place secret room
        if (Random.value <= secretRoomChance && normalRooms.Count > 0)
        {
            Vector2Int secretRoomPos = normalRooms[0];
            roomMap[secretRoomPos] = RoomGenerator.RoomType.Secret;
            normalRooms.RemoveAt(0);
        }
        
        // Place challenge room
        if (Random.value <= challengeRoomChance && normalRooms.Count > 0)
        {
            Vector2Int challengeRoomPos = normalRooms[0];
            roomMap[challengeRoomPos] = RoomGenerator.RoomType.Challenge;
            normalRooms.RemoveAt(0);
        }
        
        // Place boss room at the furthest room from the starting room
        if (normalRooms.Count > 0)
        {
            // Find the room furthest from the starting room
            Vector2Int furthestRoom = normalRooms[0];
            float maxDistance = Vector2Int.Distance(startingRoomPos, furthestRoom);
            
            for (int i = 1; i < normalRooms.Count; i++)
            {
                float distance = Vector2Int.Distance(startingRoomPos, normalRooms[i]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    furthestRoom = normalRooms[i];
                }
            }
            
            // Set as boss room
            roomMap[furthestRoom] = RoomGenerator.RoomType.Boss;
            bossRoomPos = furthestRoom;
            
            // Remove from list
            normalRooms.Remove(furthestRoom);
        }
    }
    
    private void CreateRoomObjects()
    {
        foreach (var roomEntry in roomMap)
        {
            Vector2Int gridPos = roomEntry.Key;
            RoomGenerator.RoomType roomType = roomEntry.Value;
            
            // Convert grid position to world position
            Vector2 worldPos = new Vector2(gridPos.x * roomDistance, gridPos.y * roomDistance);
            
            // Generate the room
            roomGenerator.GenerateRoom(roomType, worldPos);
        }
    }
    
    private void ConnectRooms()
    {
        // For each room in the map
        foreach (var roomEntry in roomMap)
        {
            Vector2Int gridPos = roomEntry.Key;
            
            // Check all four directions
            Vector2Int[] directions = {
                new Vector2Int(0, 1),   // North
                new Vector2Int(0, -1),  // South
                new Vector2Int(1, 0),   // East
                new Vector2Int(-1, 0)   // West
            };
            
            // Names of door objects to enable/disable
            string[] doorNames = {
                "NorthDoor",
                "SouthDoor",
                "EastDoor",
                "WestDoor"
            };
            
            // Convert grid position to world position
            Vector2 worldPos = new Vector2(gridPos.x * roomDistance, gridPos.y * roomDistance);
            
            // Find the room object
            GameObject roomObj = GameObject.Find("Room_" + worldPos.x + "_" + worldPos.y);
            if (roomObj == null)
                continue;
                
            // Check each direction
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int neighborPos = gridPos + directions[i];
                
                // If there's no room in this direction, disable the door
                if (!roomMap.ContainsKey(neighborPos))
                {
                    // Find the door object
                    Transform doorTransform = roomObj.transform.Find(doorNames[i]);
                    if (doorTransform != null)
                    {
                        doorTransform.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    
    public void AdvanceToNextFloor()
    {
        currentFloor++;
        
        if (currentFloor > floorCount)
        {
            // Player has completed all floors
            GameManager.Instance.OnGameCompleted();
        }
        else
        {
            // Generate the next floor
            GenerateDungeon();
        }
    }
    
    public Vector2 GetStartingRoomPosition()
    {
        return new Vector2(startingRoomPos.x * roomDistance, startingRoomPos.y * roomDistance);
    }
    
    public Vector2 GetBossRoomPosition()
    {
        return new Vector2(bossRoomPos.x * roomDistance, bossRoomPos.y * roomDistance);
    }
}