import {Component, Input, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";
import {AccountService} from "../_services/account.service";

@Component({
  selector: 'app-activityfeed',
  templateUrl: './activityfeed.component.html',
  styleUrls: ['./activityfeed.component.css']
})
export class ActivityfeedComponent implements OnInit {
  @Input() profileActivities : any
  constructor(private http: HttpClient, public accountService: AccountService) {}
  activities:any;
  baseUrl = environment.apiUrl
  imageUrl = environment.imageUrl


  ngOnInit(): void {
    if (this.profileActivities != null) {
      this.getActivityForProfile()
    }
    else {
      this.getActivity();
    }
  }

  getActivity(){
    this.http.get(  this.baseUrl + "messages/activity").subscribe(
      response => {
        this.activities = response;
      }, error => {
        console.log(error)}
    )
  }

  getActivityForProfile(){
    this.activities = this.profileActivities;

  }

  deleteMessage(activity){
    if(confirm("Do you wish to delete this message?")){
      this.http.put(  this.baseUrl + "messages/delete/", activity).subscribe(
        res=>{
          window.location.reload();
        },
         error => console.log(error)
      )

    }
  }

}
