import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {Router} from "@angular/router";

@Component({
  selector: 'app-topicsfeed',
  templateUrl: './topicsfeed.component.html',
  styleUrls: ['./topicsfeed.component.css']
})
export class TopicsfeedComponent implements OnInit {
  topics:any;
  alltopics:number;
  @Output() topic = new EventEmitter();
  baseUrl = environment.apiUrl
  constructor(private http: HttpClient, private router : Router) {}

  ngOnInit(): void {
    this.getTopics();
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

  emitTopic(topicName:string){
    this.router.navigate([""], {queryParams: {topic: topicName}});
    this.topic.emit(topicName)
  }

}
