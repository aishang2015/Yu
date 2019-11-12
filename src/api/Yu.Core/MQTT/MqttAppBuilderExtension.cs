using Microsoft.AspNetCore.Builder;
using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.Server;
using System;
using System.Threading.Tasks;

namespace Yu.Core.MQTT
{
    public static class MqttAppBuilderExtension
    {
        public static void UseMqtt(this IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(builder =>
            {
                builder.MapConnectionHandler<MqttConnectionHandler>("/mqtt", options =>
                {
                    options.WebSockets.SubProtocolSelector = MQTTnet.AspNetCore.ApplicationBuilderExtensions.SelectSubProtocol;
                });
            });

            app.UseMqttEndpoint("/mqtt");

            app.UseMqttServer(server =>
            {
                //server.StartedHandler = new MqttServerStartedHandlerDelegate(async args =>
                //{
                //    var msg = new MqttApplicationMessageBuilder()
                //        .WithPayload("hello mqttnet")
                //        .WithTopic("start");

                //    while (true)
                //    {
                //        try
                //        {
                //            await server.PublishAsync(msg.Build());
                //            msg.WithPayload("Mqtt Server is alive ");
                //        }
                //        catch (Exception e)
                //        {
                //            Console.WriteLine(e);
                //        }
                //        finally
                //        {
                //            await Task.Delay(TimeSpan.FromSeconds(2));
                //        }
                //    }
                //});

                server.StoppedHandler = new MqttServerStoppedHandlerDelegate(async args =>
                {
                    await Task.CompletedTask;
                });

                server.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(async args =>
                 {
                     await Task.CompletedTask;
                 });


                server.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(async args =>
                {
                    await Task.CompletedTask;
                });

                server.UseApplicationMessageReceivedHandler(async args =>
                {
                    //var item = $"Timestamp: {DateTime.Now:O} | Topic: {args.ApplicationMessage.Topic} | Payload: {args.ApplicationMessage.ConvertPayloadToString()} | QoS: {args.ApplicationMessage.QualityOfServiceLevel}\r";
                    await Task.CompletedTask;
                });
            });
        }
    }
}
