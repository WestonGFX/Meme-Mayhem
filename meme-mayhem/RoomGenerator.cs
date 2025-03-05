    // Continuing from previous code...
    
    private void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0)
            return;
            
        // Calculate a random position that's not too close to doors
        Vector2 spawnPos;
        bool validPosition = false;
        
        do {
            float x = Random.Range(-roomDimensions.x * 0.4f, roomDimensions.x * 0.4f);
            float y = Random.Range(-roomDimensions.y * 0.4f, roomDimensions.y * 0.4f);
            spawnPos = new Vector2(x, y);
            
            // Check if position is far enough from doors
            float minDistToDoor = 2f;
            validPosition = 
                Vector2.Distance(spawnPos, new Vector2(0, roomDimensions.y * 0.5f)) > minDistToDoor &&
                Vector2.Distance(spawnPos, new Vector2(0, -roomDimensions.y * 0.5f)) > minDistToDoor &&
                Vector2.Distance(spawnPos, new Vector2(roomDimensions.x * 0.5f, 0)) > minDistToDoor &&
                Vector2.Distance(spawnPos, new Vector2(-roomDimensions.x * 0.5f, 0)) > minDistToDoor;
                
        } while (!validPosition);
        
        // Spawn a random enemy
        int enemyIndex = Random.Range(0, enemyPrefabs.Count);
        Instantiate(enemyPrefabs[enemyIndex], spawnPos, Quaternion.identity, roomContainer);
    }
    
    private void SpawnItem()
    {
        if (itemPrefabs.Count == 0)
            return;
            
        // Calculate random position
        float x = Random.Range(-roomDimensions.x * 0.3f, roomDimensions.x * 0.3f);
        float y = Random.Range(-roomDimensions.y * 0.3f, roomDimensions.y * 0.3f);
        Vector2 spawnPos = new Vector2(x, y);
        
        // Spawn random item
        int itemIndex = Random.Range(0, itemPrefabs.Count);
        Instantiate(itemPrefabs[itemIndex], spawnPos, Quaternion.identity, roomContainer);
    }
}