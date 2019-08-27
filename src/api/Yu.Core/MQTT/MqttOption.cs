namespace Yu.Core.MQTT
{
    public class MqttOption
    {
        // Ip地址
        public string IpAddress { get; set; }

        // 端口号
        public int Port { get; set; }

        // 超时时间（秒）
        public int Timeout { get; set; }

        // 用户名
        public string UserName { get; set; }

        // 密码
        public string Password { get; set; }
    }
}
