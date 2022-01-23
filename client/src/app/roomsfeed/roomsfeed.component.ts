import {Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-roomsfeed',
  templateUrl: './roomsfeed.component.html',
  styleUrls: ['./roomsfeed.component.css']
})
export class RoomsfeedComponent implements OnInit, OnChanges {
  rooms: any
  ar: Array<string>;
  @Input() profileRooms: any
  @Output() totalRooms = new EventEmitter<number>();
  imageUrl = environment.imageUrl;
  baseUrl = environment.apiUrl;
  _searchroom: string

  // This will trigger on Input change
  @Input() searchroom : string
  @Input() topicName : string



  constructor(private http: HttpClient, private router: Router, private activatedRoute: ActivatedRoute) {
  }

  ngOnChanges(changes: SimpleChanges): void {
    console.log(changes['searchroom'])
    if (changes['searchroom']){
      this._searchroom = changes['searchroom'].currentValue
      this.getRoomsBySearch()
    } else if (this.topicName != null){
      this.getRoomsByTopic()
    }
  }

  ngOnInit(): void {
    this.activatedRoute.queryParams.subscribe((params) => {
      if (params['searchroom'] != null) {
        this._searchroom = params['searchroom'];
        this.getRoomsBySearch()
      }else if (params['topic'] != null) {
        this.topicName = params['topic']
        this.getRoomsByTopic()
      } else  {
        if (this.profileRooms){
          this.rooms = this.profileRooms;
        } else {
          this.getRooms();
        }
      }
    })


  }

  getRooms(){
      this.http.get( this.baseUrl + "room/").subscribe(response => {
        this.rooms = response;
        this.totalRooms.emit(this.rooms.length);
      }, error => {
        console.log(error);
      })

  }

  getRoomsBySearch() {
    if (this._searchroom != "" && this._searchroom != null) {
      this.http.get(this.baseUrl + "search/" + this._searchroom).subscribe(
        res => {
          this.rooms = res;
          this.totalRooms.emit(this.rooms.length);
        }, error => {
          console.log(error);
        }
      )
    }
  }

  getRoomsByTopic(){
    console.log(this.topicName)
    console.log("run")
    this.http.get( this.baseUrl + "room/topic/" + this.topicName).subscribe(
      res=> this.rooms = res, error => console.log(error)
    )

  }

  // getRoomsBySearch(){
  //   console.log("test")
  //   // console.log(room_search)
  //   console.log(this.searchroom)
  //   this.http.get("https://localhost:7241/api/search/" + this.searchroom).subscribe(
  //   res=> this.rooms = res
  //   )
  // }


}
