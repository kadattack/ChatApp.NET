import {NgModule} from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import {HomeComponent} from "./home/home.component";
import {ProfileComponent} from "./profile/profile.component";
import {LoginComponent} from "./login/login.component";
import {RegisterComponent} from "./register/register.component";
import {RoomchatComponent} from "./roomchat/roomchat.component";
import {AuthGuard} from "./_guards/auth.guard";
import {EditUserComponent} from "./edit-user/edit-user.component";
import {SettingsComponent} from "./settings/settings.component";
import {CreateroomComponent} from "./createroom/createroom.component";
import {EditRoomComponent} from "./edit-room/edit-room.component";

const routes: Routes = [
  {path:"", component: HomeComponent},
  {path:"login", component: LoginComponent},
  {path:"register", component: RegisterComponent},

  {path:"profile", component: ProfileComponent, canActivate: [AuthGuard]},
  {path:"profile/edit-user", component: EditUserComponent, canActivate: [AuthGuard]},
  {path:"profile/settings", component: SettingsComponent, canActivate: [AuthGuard]},
  {path:"create-room", component: CreateroomComponent, canActivate: [AuthGuard]},
  {path:"edit-room", component: EditRoomComponent, canActivate: [AuthGuard]},


  {path:"profile/:username", component: ProfileComponent, canActivate: [AuthGuard]},
  {path:"room/:roomname", component: RoomchatComponent, canActivate: [AuthGuard]},
  // {path:"**", component: HomeComponent, pathMatch: "full"},



];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
