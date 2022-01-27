import {
  AfterViewInit,
  Component,
  EventEmitter,
  Inject,
  Input,
  OnInit,
  Output,
  Renderer2,
  ViewChild
} from '@angular/core';
import {AccountService} from "../_services/account.service";
import {thBeLocale} from "ngx-bootstrap/chronos";
import { DOCUMENT } from '@angular/common';
import {Observable} from "rxjs";
import {User} from "../_models/user";
import {ActivatedRoute, Router, RouterLinkActive} from "@angular/router";
import {environment} from "../../environments/environment";
import {HttpClient} from "@angular/common/http";
import {map} from "rxjs/operators";



@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit, AfterViewInit {

  rooms :any
  user: any;
  droppped: boolean = false;
  imageUrl = environment.imageUrl;
  searchroom :string;
  @Output() roomSearchEmit = new EventEmitter<string>()

  constructor(private http: HttpClient, public accountService: AccountService, private renderer: Renderer2, private router: Router, private route : ActivatedRoute) {
    accountService
  }

  logout(){
    this.accountService.logout();
    if (this.router.url == "/"){
      window.location.reload()
    } else {
      this.router.navigateByUrl("");
    }
  }


  ngOnInit(): void {

  }
  ngAfterViewInit(): void {
    // this.renderer.
    // console.log(this.dropdownbutton)
    // console.log(this.dropdownbutton.nativeElement)
    // if (this.dropdownbutton) {
    //   this.dropdownbutton.nativeElement.addEventListener("click", () => {
    //     this.dropdownmenu.nativeElement.classList.toggle("show");
    //   });
    // }
  }

    // // Upload Image
    // const photoInput:any = document.querySelector("#avatar");
    // const photoPreview:any = document.querySelector("#preview-avatar");
    // if (photoInput)
    //   photoInput.onchange = () => {
    //     const [file] = photoInput.files;
    //     if (file) {
    //       photoPreview.src = URL.createObjectURL(file);
    //     }
    //   };
    //
    // // Scroll to Bottom
    // const conversationThread = document.querySelector(".room__box");
    // if (conversationThread) conversationThread.scrollTop = conversationThread.scrollHeight;


  showNavDropdown(){
    this.droppped = !this.droppped;
  }


  async searchRoomName(){
    await this.roomSearchEmit.emit(this.searchroom);
    // this.router.navigateByUrl("")
    this.router.navigate([''], {queryParams: {searchroom: this.searchroom}});


  }

  reload(event){
    var username = event.target.innerText
    username = username.replace('@','').trim()
    this.router.navigateByUrl("/", {skipLocationChange: true}).then(()=>
      this.router.navigateByUrl("/profile/" + username)
  );
  }


}


