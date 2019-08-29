using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MQTTnet.AspNetCore;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Net;

namespace Yu.Core.MQTT
{
    public static class MqttServiceCollectionExtension
    {
        public static void AddMqtt(this IServiceCollection services, IConfiguration configuration)
        {
            // 注册配置文件
            services.Configure<MqttOption>(configuration.GetSection("MqttOption"));

            // 访问配置文件
            var mqttOption = services.BuildServiceProvider().GetRequiredService<IOptions<MqttOption>>().Value;

            // mqtt配置构建
            var option = new MqttServerOptionsBuilder()
                .WithDefaultEndpointBoundIPAddress(IPAddress.Parse(mqttOption.IpAddress))
                .WithDefaultEndpointPort(mqttOption.Port)
                .WithDefaultCommunicationTimeout(TimeSpan.FromSeconds(mqttOption.Timeout))
                .WithConnectionValidator(context =>
                {
                    if (context.Username != mqttOption.UserName || context.Password != mqttOption.Password)
                    {
                        context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                    }
                    context.ReasonCode = MqttConnectReasonCode.Success;
                }).Build();

            // IMqttServer被注册到容器，可以取得并用其发布消息
            services.AddHostedMqttServer(option)
                .AddMqttConnectionHandler()
                .AddConnections();
        }
    }
}
