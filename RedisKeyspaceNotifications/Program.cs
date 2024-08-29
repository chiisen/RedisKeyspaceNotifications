using StackExchange.Redis;

class Program
{
    static void Main()
    {
        // 建立 Redis 連接
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");

        // 創建一個訂閱者
        var subscriber = redis.GetSubscriber();

        IDatabase db = redis.GetDatabase(0);
        string valueFirst = db.StringGet("eventnews");
        Console.WriteLine($"'eventnews': {valueFirst}");

        // 設置 Keyspace Notifications，監聽 key 為 "eventnews" 的事件
        subscriber.Subscribe("__keyspace@0__:eventnews", (channel, message) =>
        {
            if (message == "set")
            {
                // key 的值被設置，獲取並列印新的值
                db = redis.GetDatabase(0);
                string value = db.StringGet("eventnews");
                Console.WriteLine($"key 為 eventnews 的值已被設置為: {value}");
            }
        });

        Console.WriteLine("正在監聽 key 為 eventnews 的值變化，按 Enter 鍵退出。");
        Console.ReadLine();

        // 關閉 Redis 連接
        redis.Close();
    }
}