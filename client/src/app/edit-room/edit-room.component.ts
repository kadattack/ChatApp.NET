import { Component, OnInit } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {ActivatedRoute, Router} from "@angular/router";
import {environment} from "../../environments/environment";

@Component({
  selector: 'app-edit-room',
  templateUrl: './edit-room.component.html',
  styleUrls: ['./edit-room.component.css']
})
export class EditRoomComponent implements OnInit {

  baseUrl = environment.apiUrl

  room = {id:0, roomName:'', topicName:'', description:''}
  constructor(private http: HttpClient, private route : ActivatedRoute, private router : Router) { }



  ngOnInit(): void {
    this.getInfoOfRoom()
  }

  getInfoOfRoom(){
    this.route.queryParams.subscribe(params=> {
      this.http.get(this.baseUrl + "room/id/" + params.id).subscribe((res:any)=> {
        this.room.id = params.id
        this.room.roomName = res.roomName
        this.room.topicName = res.topicName
        this.room.description = res.description
      })

    })

  }

  editRoom(){
    this.http.put(this.baseUrl + "room/edit", this.room).subscribe(res=> this.router.navigateByUrl(""), error => console.log(error))
  }
}
