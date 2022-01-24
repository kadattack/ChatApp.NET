import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {environment} from "../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class ImageService {

  baseUrl = environment.apiUrl;

  constructor(private  http: HttpClient) { }

  getImage(imageurl:string){
      return this.http.get(this.baseUrl + "image/" + imageurl)
  }
}
