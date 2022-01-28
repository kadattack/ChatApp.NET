import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {User} from "../_models/user";

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl
  private hubConnection:HubConnection;
  constructor() {
  }

  creatHubConnection( user : User){
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl + 'presence', {
      accessTokenFactory(): string | Promise<string> {
        return user.token
      }
    }).withAutomaticReconnect().build()
      // Uncomplete: this only writes in console. No implementation on server side yet.
      this.hubConnection.start().catch((error) => console.log(error));

      this.hubConnection.on("UserIsOnline", username =>{
        console.log(`user ${username} has connected`)
      })
      this.hubConnection.on("UserIsOffline", username =>{
        console.log(`user ${username} has disconnected`)
      })


  }

  stopHubConnection(){
    this.hubConnection.stop().catch(error=> console.log(error))
  }
}
