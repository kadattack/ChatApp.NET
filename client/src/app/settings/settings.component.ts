import {AfterViewInit, AfterViewChecked, Component, OnInit, ViewChild} from '@angular/core';
import {User} from "../_models/user";
import {AccountService} from "../_services/account.service";
import {ProfilesService} from "../_services/profiles.service";
import {take} from "rxjs/operators";
import {ImageService} from "../_services/image.service";
import {environment} from "../../environments/environment";
import {Router} from "@angular/router";

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})




export class SettingsComponent implements OnInit, AfterViewChecked {
  profile: any;
  user: any;
  cyrb53:any
  @ViewChild('editForm') editForm;
  @ViewChild('photoInput') photoInput;
  // @ViewChild('photoPreview') photoPreview;
  photoPreview:any
  imageUrl = environment.imageUrl;
  form:any

  constructor(private accountService : AccountService, private profileService : ProfilesService, private router :Router) {
    accountService.currentUser$.subscribe(user => {this.user = user;
      }
    );
  }

  ngAfterViewChecked(): void {
    // Hash function
    this.cyrb53 = function(str, seed = 0) {
      let h1 = 0xdeadbeef ^ seed, h2 = 0x41c6ce57 ^ seed;
      for (let i = 0, ch; i < str.length; i++) {
        ch = str.charCodeAt(i);
        h1 = Math.imul(h1 ^ ch, 2654435761);
        h2 = Math.imul(h2 ^ ch, 1597334677);
      }
      h1 = Math.imul(h1 ^ (h1>>>16), 2246822507) ^ Math.imul(h2 ^ (h2>>>13), 3266489909);
      h2 = Math.imul(h2 ^ (h2>>>16), 2246822507) ^ Math.imul(h1 ^ (h1>>>13), 3266489909);
      return (4294967296 * (2097151 & h2) + (h1>>>0)).toString();
    };

    this.photoPreview = document.querySelector("#preview-avatar");

  }



  ngOnInit(): void {
    this.loadProfile()
  }

  loadProfile(){
    this.profileService.getProfile(this.user.userName).subscribe(profile=>{
      this.profile = profile;
      this.profile.avatarImageObject = null

    })
  }

  processFile(imageInput:any){
    const file: File = imageInput.files[0];
    const reader = new FileReader();
    reader.readAsDataURL(file);

    let avatarImageObject = {imageData: null , url: null }

    if (this.photoInput) {
      this.editForm.control.markAsDirty()
      if (file) {
        reader.onload = function () {
          avatarImageObject.imageData = reader.result;
          avatarImageObject.url = this.cyrb53(reader.result);
          this.profile.avatarImageObject = avatarImageObject;

        }.bind(this);
        this.photoPreview.src = URL.createObjectURL(file);

      }

    }
  }


  updateProfile(){
    console.log(this.profile)
    this.profileService.updateProfile(this.profile).subscribe(()=>{
      var user = JSON.parse(localStorage['user'])
      if (this.profile.avatarImageObject) user.avatarUrl = this.profile.avatarImageObject.url
      else user.avatarUrl = this.profile.avatarUrl
      this.accountService.setCurrentUser(user)
      localStorage.setItem('user', JSON.stringify(user));
      this.router.navigateByUrl("");
    })
  }



}
