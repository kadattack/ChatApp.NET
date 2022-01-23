import {Component, ElementRef, OnInit, ViewChild} from '@angular/core';
import {createroom} from "../_models/createroom";
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import { Router} from "@angular/router";

@Component({
  selector: 'app-createroom',
  templateUrl: './createroom.component.html',
  styleUrls: ['./createroom.component.css']
})
export class CreateroomComponent implements OnInit {
  baseUrl = environment.apiUrl
  @ViewChild('errorResponse') textP :ElementRef
  newRoom: createroom = {roomName:"", topicName:"", description:""}
  constructor(private http: HttpClient, private router : Router) { }

  ngOnInit(): void {
  }

  createRoom(){
    this.http.put(this.baseUrl + "room", this.newRoom).subscribe(
      res=> {console.log(res); this.router.navigateByUrl("")},  error => {console.log(error); this.textP.nativeElement.innerText = "Room in that Topic already exists."}
    )
  }

}
