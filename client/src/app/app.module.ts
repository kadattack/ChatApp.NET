import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import {HTTP_INTERCEPTORS, HttpClientModule} from "@angular/common/http";
import { ActivityfeedComponent } from './activityfeed/activityfeed.component';
import { RoomsfeedComponent } from './roomsfeed/roomsfeed.component';
import { TopicsfeedComponent } from './topicsfeed/topicsfeed.component';
import { NavbarComponent } from './navbar/navbar.component';
import { ProfileComponent } from './profile/profile.component';
import {FormsModule} from "@angular/forms";
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RoomchatComponent } from './roomchat/roomchat.component';
import {JwtInterceptor} from "./_interceptors/jwt.interceptor";
import { EditUserComponent } from './edit-user/edit-user.component';
import { SettingsComponent } from './settings/settings.component';
import { CreateroomComponent } from './createroom/createroom.component';
import { EditRoomComponent } from './edit-room/edit-room.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    ActivityfeedComponent,
    RoomsfeedComponent,
    TopicsfeedComponent,
    NavbarComponent,
    ProfileComponent,
    LoginComponent,
    RegisterComponent,
    RoomchatComponent,
    EditUserComponent,
    SettingsComponent,
    CreateroomComponent,
    EditRoomComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    BrowserAnimationsModule
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
