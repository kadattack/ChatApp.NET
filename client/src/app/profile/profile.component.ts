import { Component, OnInit } from '@angular/core';
import {AccountService} from "../_services/account.service";
import {map} from "rxjs/operators";
import {Profile} from "../_models/profile";
import {ProfilesService} from "../_services/profiles.service";
import {ActivatedRoute} from "@angular/router";
import {environment} from "../../environments/environment";

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit {
  profile : any
  imageUrl = environment.imageUrl
  cabEdit: boolean
  constructor(public accountService : AccountService, private profileService : ProfilesService, private route: ActivatedRoute) {
    accountService
  }

  ngOnInit(): void {
    if (this.accountService.currentUser$){
      this.accountService.currentUser$.subscribe(res=>
      {if (res.userName == this.route.snapshot.paramMap.get('username')){
        this.cabEdit = true;
      }}
      )
    }

    this.getProfileOf()

  }

  getProfileOf(){
    this.profileService.getProfile(this.route.snapshot.paramMap.get('username')).subscribe(
      (response:any) => {this.profile = response; console.log(this.profile); return response}
    )
  }

}
