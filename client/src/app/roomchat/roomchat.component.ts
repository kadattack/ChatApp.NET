import {AfterContentInit, AfterViewChecked, AfterViewInit, Component, Inject, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {Roomchat} from "../_models/roomchat";
import {map} from "rxjs/operators";
import {User} from "../_models/user";
import {ProfilesService} from "../_services/profiles.service";
import {Profile} from "../_models/profile";
import {ActivatedRoute, Router} from "@angular/router";
import {AccountService} from "../_services/account.service";
import {Message} from "../_models/message";
import {Room} from "../_models/room";
import {environment} from "../../environments/environment";

@Component({
  selector: 'app-roomchat',
  templateUrl: './roomchat.component.html',
  styleUrls: ['./roomchat.component.css']
})
export class RoomchatComponent implements OnInit, AfterViewChecked {
  // @Inject(chatRoomName:string)
  conversationThread:any;
  room:Room ;
  imageUrl = environment.imageUrl
  baseUrl = environment.apiUrl

  profile: Profile;
  message: Message = {avatarUrl:'',body:'',messageIsInRoom:'',created:'',userCreated:'',updated:'',topicName:''};
  constructor(private http: HttpClient, private route: ActivatedRoute, private accountService: AccountService, private router: Router) {

  }

  ngOnInit(): void {
    this.getRoom()
    console.log(this.baseUrl)
    console.log(this.imageUrl)


  }

  async getRoom() {
    this.route.queryParams.subscribe(param=> {
      console.log(param.topic)
      var createroomDto = {RoomName: this.route.snapshot.paramMap.get('roomname'), TopicName: param.topic, Description: ''}
      this.http.post<Room>( this.baseUrl + "room/", createroomDto).subscribe(
        ((response:Room) =>{
          console.log(response)
          this.room = response
          return response
        })
      )
    })

  }

  sendMessage(){
    this.accountService.currentUser$.subscribe(user=> {
      this.message.userCreated = user.userName
      this.message.messageIsInRoom = this.route.snapshot.paramMap.get('roomname')
      this.http.put(this.baseUrl +"messages/", this.message).subscribe(res=> {
        this.message.body = "";
        this.getRoom();
        if (this.conversationThread) this.conversationThread.scrollTo(0,document.querySelector(".threads").scrollHeight)

        }
        ,error => console.log(error))
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



}
