import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr'
import { UriConstant } from '../constants/uri-constant';
import { HubConnectionState } from '@aspnet/signalr';

@Injectable({
    providedIn: 'root'
})

export class SignalrService {

    private connection = new signalR.HubConnectionBuilder().withUrl(UriConstant.ServerUri + 'hub',
        { accessTokenFactory: () => localStorage.getItem('yu_auth_token') }).configureLogging(signalR.LogLevel.Error).build();

    constructor() {
        console.log("start");
        this.init();
    }

    init(): void {
        if (!this.connection || this.connection.state == HubConnectionState.Disconnected) {
            this.connection = new signalR.HubConnectionBuilder().withUrl(UriConstant.ServerUri + 'hub',
                { accessTokenFactory: () => localStorage.getItem('yu_auth_token') }).configureLogging(signalR.LogLevel.Error).build();
        }
        this.connection.start().then((i) => { }, error => { console.log(error); }).catch(e => console.log(e));
    }

    // 停止连接
    stop() {
        this.connection.stop();
        this.connection = null;
    }

    // 往服务器发送信息
    // 此方法的实现需要和客户端的方法定义形式一致
    // 例如我想要调用服务器hub的public async Task GetInfo(string info)方法
    // 就需要定义成this.connection.send(method, args);然后对它进行方法的封装。
    private sendToServer(method, ...args: any[]) {
        this.connection.send(method, args);
    }

    // 绑定服务器消息处理方法
    addEventHandler(eventName: string, newMethod: (...args: any[]) => void) {
        this.connection.off(eventName);
        this.connection.on(eventName, newMethod);
    }

    // 删除服务器消息处理方法
    removeEventHandler(eventName: string, ) {
        this.connection.off(eventName);
    }

}