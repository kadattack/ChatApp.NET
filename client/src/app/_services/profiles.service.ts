import { Injectable } from '@angular/core';
import {environment} from "../../environments/environment";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {map} from "rxjs/operators";
import {AccountService} from "./account.service";
import {Profile} from "../_models/profile";
import {ActivatedRoute} from "@angular/router";
import {Observable} from "rxjs";



@Injectable({
  providedIn: 'root'
})
export class ProfilesService {
  baseUrl = environment.apiUrl;
  constructor(private  http: HttpClient, private accountService: AccountService, private route: ActivatedRoute ) { }

  getProfile(username:string){
    // let user = this.accountService.currentUser$
    // let model = {username: user. }
    // let model = JSON.parse(localStorage.getItem("user"))
    return this.http.get(this.baseUrl + "users/" + username)
  }

  updateProfile(profile){
    return this.http.put(this.baseUrl + 'users', profile);
  }

  public uploadImage(image: File): Observable<Response> {
    const formData = new FormData();

    formData.append('image', image);

    return this.http.post<Response>('/api/v1/image-upload', formData);
  }


}
