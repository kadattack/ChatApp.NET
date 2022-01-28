import {AfterContentInit, AfterViewChecked, AfterViewInit, Component, Inject, OnDestroy, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Roomchat} from "../_models/roomchat";
import {map, take} from "rxjs/operators";
import {User} from "../_models/user";
import {ProfilesService} from "../_services/profiles.service";
import {Profile} from "../_models/profile";
import {ActivatedRoute, Router} from "@angular/router";
import {AccountService} from "../_services/account.service";
import {Message} from "../_models/message";
import {Room} from "../_models/room";
import {environment} from "../../environments/environment";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";

@Component({
  selector: 'app-roomchat',
  templateUrl: './roomchat.component.html',
  styleUrls: ['./roomchat.component.css']
})
export class RoomchatComponent implements OnInit, AfterViewChecked, OnDestroy {
  // @Inject(chatRoomName:string)
  hubUrl = environment.hubUrl;
  conversationThread:any;
  room:Room ;
  topic:string;
  imageUrl = environment.imageUrl
  baseUrl = environment.apiUrl
  private hubConnection: HubConnection;
  profile: Profile;
  message: Message = {avatarUrl:'',body:'',messageIsInRoom:'',created:'',userCreated:'',updated:'',topicName:''};
  constructor(private http: HttpClient, private route: ActivatedRoute, private accountService: AccountService, private router: Router) {

  }

  ngOnInit(): void {
    this.getRoom()
  }

  createHubConnection(user: User, room: Room){
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl + 'message?roomname=' + room.roomName + "&topicname=" + room.roomTopic, {
      accessTokenFactory: () => user.token
    }).withAutomaticReconnect().build()
    this.hubConnection.start().catch(error => {console.log(error)});


    this.hubConnection.on("ReceiveRoomMessage", res =>{
      this.room = res.value
    })

    this.hubConnection.on("NewMessage", res=> {
      this.room.messages.push(res)
      if (this.conversationThread) this.conversationThread.scrollTo(0,document.querySelector(".threads").scrollHeight)
    })
  }

  stopHubConnection(){
    this.hubConnection.stop();
  }


  async getRoom() {
    this.route.queryParams.subscribe(param=> {
      this.topic = param.topic
      this.message.topicName = param.topic
      this.message.messageIsInRoom = this.route.snapshot.paramMap.get('roomname')
      var createroomDto = {RoomName: this.route.snapshot.paramMap.get('roomname'), TopicName: param.topic, Description: ''}
      this.http.post<Room>( this.baseUrl + "room/", createroomDto).subscribe(
        ((response:Room) =>{
          this.accountService.currentUser$.pipe(take(1)).subscribe((user:User)=>{
              this.createHubConnection(user, response)
            }
          )
          return response
        })
      )
    })

  }

  async sendMessage(){
    console.log()
    this.accountService.currentUser$.subscribe(user=> {
      this.message.userCreated = user.userName
      this.hubConnection.invoke("SendMessage", this.message).catch(error => console.log(error))

      // this.http.put(this.baseUrl +"messages/", this.message).subscribe(res=> {
      //   this.message.body = "";
      //   this.getRoom();
      //   if (this.conversationThread) this.conversationThread.scrollTo(0,document.querySelector(".threads").scrollHeight)
      //
      //   }
      //   ,error => console.log(error))
    })
  }

  ngAfterViewChecked(): void {
    this.conversationThread = document.querySelector(".threads");
    if (this.conversationThread) this.conversationThread.scrollTo(0,document.querySelector(".threads").scrollHeight)
  }

  deleteMessage(message){
    if(confirm("Do you wish to delete this message?")){
      this.http.put(  this.baseUrl + "messages/delete/", message).subscribe(
        res=>{
          this.ngOnInit()
        },
        error => console.log(error)
      )
    }
  }

  deleteRoom(){
    console.log(this.room.id)
    if(confirm("Are you sure you wish to delete this Room")){
      this.http.get(this.baseUrl + "room/delete/" + this.room.id ).subscribe(res=> this.router.navigateByUrl(""), error => console.log(error))
    }
  }

  ngOnDestroy(): void {
    this.stopHubConnection()
  }





}
