import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  rooms: any;
  allRooms: number = 0;
  activities: any;
  topics: any;
  alltopics: number;
  @Input() roomName :string
  @Input() topicName :string
  forwardEmitTopicName : string
  forwardEmitRoomName: string
  baseUrl = environment.apiUrl

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.getActivity();
    this.getTopics()
  }


  getRooms(noRooms:number){
    // this.http.get("https://localhost:7241/api/room/").subscribe(response => {
    //   this.rooms = response;
    // }, error => {
    //   console.log(error);
    // })
    // this.allRooms = RoomComponent.
    this.allRooms = noRooms;
  }

  getActivity(){
    this.http.get(this.baseUrl + "messages/activity").subscribe(
      response => {
        this.activities = response;
      }, error => {
        console.log(error)}
    )
  }

  getTopics(){
    this.http.get(this.baseUrl + "topics/").subscribe(
      response =>{
        this.topics = response;
        var mno = 0;
        for (let i = 0; i < this.topics.length; i++) {
          mno += this.topics[i]["messagesNo"]
        }
        this.alltopics = mno;
      }, error => {
        console.log(error)
      }
    )
  }

  searchRoomName(roomname:string){
    if (roomname == null){
      roomname = ""
    }
    this.forwardEmitRoomName = roomname
  }

  selectTopic(topic:string){
    this.forwardEmitTopicName = topic;
  }


}
