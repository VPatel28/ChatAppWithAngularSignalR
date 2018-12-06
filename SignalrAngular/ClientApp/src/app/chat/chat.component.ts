import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ChatService } from '../chat.service';
import { first } from 'rxjs/operators';
import { ChatMessage } from '../ChatMessage';
import { HubConnection } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit {
  private _hubConnection: HubConnection;
  public userName: string;
  public nickName: string;
  public id: number;
  public resid: number;
  public unreadmsg: number;
  public currentuser: ChatMessage;
  public respectiveuser: ChatMessage;
  openchatwindow: boolean = false;
  element: HTMLElement;
  public msghtm: string = "";
  public msg: string = "";
  public chathisto: string = "";

  public chatUserList: ChatMessage[];
  constructor(public _router: Router, public chatService: ChatService, public chatMsg: ChatMessage) {
    this.currentuser = new ChatMessage();
    this.respectiveuser = new ChatMessage();
    this.checkIfUserValid();
    this.createConnection();
    this.registerOnServerEvents(this._hubConnection, chatService);
    this.startConnection();
  }

  checkIfUserValid() {
    if (localStorage.length > 0) {
      debugger;
      let item = JSON.parse(localStorage.getItem('User'));
      if (item == null || item == '' || item == undefined) {
        localStorage.clear();
        this._router.navigate(['/']);
      }
      else {
        if (item.id > 0) {
          this.nickName = item.nickName;
          this.id = item.id;
          this.userName = item.userName;
          this.currentuser.userName = this.userName;
          this.currentuser.nickName = this.nickName;
          this.currentuser.id = this.id;
        }
        else {
          localStorage.clear();
          this._router.navigate(['/']);
        }
      }
    } else {
      localStorage.clear();
      this._router.navigate(['/']);
    }
  }


  ngOnInit() {
  }



  Logout() {

    this.chatService.UpdateUserDetails(this.currentuser)
      .pipe(first())
      .subscribe(
        data => {
          let key = 'User';
          localStorage.removeItem(key);
          localStorage.clear();
          this._router.navigate(['/']);
        },
        error => {
          if (error.status == "404")
            alert("Invalid UserName and Password");

          console.log(error + ' while posting data ' + this.currentuser);

        });
  }



  sendChatMessage(message: ChatMessage) {
    this._hubConnection.invoke('SendMessage', message);
  }

  private createConnection() {
    this._hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('/appHub')
      .configureLogging(signalR.LogLevel.Information)
      .build();
  }

  private startConnection() {
    this._hubConnection
      .start() // <- Failure here
      .then(() => {
        debugger;
        this._hubConnection.invoke("UserConnected", this.id);
      })
      .catch(err => { console.log("INVOKE FAILED"); console.error(err); });
  }

  private registerOnServerEvents(_hubConnection: HubConnection, chatService: ChatService): void {

    this._hubConnection.on('ConnectedUser', (data: string) => {
      this._hubConnection.invoke("FetchUsers");
    });

    this._hubConnection.on("bindUsers", (data: any[]) => {
      debugger;
      this.chatUserList = data.filter(data => data.nickName != this.nickName);
    });

    this._hubConnection.on("HistorySaved", (currentuseid: number, respuseid: number) => {
      debugger;
      if (this.currentuser.id == currentuseid || respuseid == this.respectiveuser.id) {
        this._hubConnection.invoke("CreateGroup", currentuseid, respuseid)
          .catch(function (err) {
            return console.error(err.toString());
          })
      }
      else if (this.currentuser.id == respuseid) {
        this._hubConnection.invoke("SaveNonActiveGroup", respuseid, currentuseid)
          .catch(function (err) {
            return console.error(err.toString());
          })
      }
    });



    this._hubConnection.on("GroupCreated", (currentuseid: number, respectiveuseid: number, grp: string) => {

      if (currentuseid == this.currentuser.id) {
        this.currentuser.grpName = grp;
        debugger;
        this.openchatwindow = true;
        this.respectiveuser.id = respectiveuseid;
        this._hubConnection.invoke("FetchHistory", currentuseid, respectiveuseid)
          .then(function () { })
          .catch(function (err) {
            return console.error(err.toString());
          })
      }
    });



    this._hubConnection.on("ShowMessage", (curuserid: number, respecuserid: number, msg:
      string, grp: string, respectusern: string, curreusern: string) => {
      debugger;
      var check = 0;
      console.log(curuserid + "===>" + respecuserid)
      if (this.currentuser.grpName == grp) {
        check = 1;
        this.unreadmsg = 0;
      }

      if (check == 1) {
        var msg = msg.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var encodedMsg;
        if (curuserid == this.currentuser.id) {
          encodedMsg = "Me" + " : " + msg;
        }
        else
          encodedMsg = this.respectiveuser.nickName + " : " + msg;

        var li = document.createElement("li") as HTMLElement;
        li.textContent = encodedMsg;

        this.element = document.getElementById('messagesList') as HTMLElement;
        this.element.appendChild(li);
        if (this.element == null)
          this.msghtm = ""
        else
          this.msghtm = this.element.innerHTML;



        this.element = document.getElementById('chathistory') as HTMLElement;
        if (this.element == null)
          this.chathisto = ""
        else
          this.chathisto = this.element.innerHTML;


        this.msg = "";

        this._hubConnection.invoke("UpdateHistory", curuserid, respecuserid, respecuserid, this.chathisto + this.msghtm, 0)
          .then(function () { })
          .catch(function (err) {
            return console.error(err.toString());
          })
      }
      else {
        
        if (this.currentuser.id == curuserid || this.currentuser.id == respecuserid) {
          this.unreadmsg = this.unreadmsg + 1;
          var msg = msg.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
          var encodedMsg;
          if (curuserid == this.currentuser.id) {
            encodedMsg = "Me" + " : " + msg;
          }
          else
            encodedMsg = curreusern + " : " + msg;

          alert(encodedMsg);

          var ul = document.createElement("ul") as HTMLElement;
          var li = document.createElement("li") as HTMLElement;
          li.textContent = encodedMsg;
          ul.appendChild(li);
          this._hubConnection.invoke("UpdateHistory", respecuserid, curuserid, curuserid, ul.innerHTML, 1)
            .then(function () { })
            .catch(function (err) {
              return console.error(err.toString());
            })
        }
      }

    });





    this._hubConnection.on("OpenChatWi", (User: any[], respusertid: number, history: string) => {

      debugger;

      this.msg = "";
      if (history == null)
        history = "";

      this.chathisto = history;

      this.element = document.getElementById('chathistory') as HTMLElement;
      if (this.element != null)
        this.element.innerHTML = history;



      this.element = document.getElementById('messagesList') as HTMLElement;
      if (this.element != null)
        this.element.innerHTML = ""


      if (respusertid == this.respectiveuser.id)
        this.unreadmsg = 0;

    });
  }


  openchat(user: ChatMessage) {
    debugger;

    if (this.respectiveuser.id == undefined)
      this.respectiveuser.id = 0;

    var currentolduser = this.respectiveuser;
    this.respectiveuser = user;
    this.resid = user.id;
    this.openchatwindow = true;


    this.element = document.getElementById('messagesList') as HTMLElement;
    if (this.element == null)
      this.msghtm = ""
    else
      this.msghtm = this.element.innerHTML;

    this.element = document.getElementById('chathistory') as HTMLElement;
    if (this.element == null)
      this.chathisto = ""
    else
      this.chathisto = this.element.innerHTML;

    this._hubConnection.invoke("SaveHistory", this.currentuser.id, currentolduser.id, this.respectiveuser.id, this.chathisto + this.msghtm)
      .catch(function (err) {
        return console.error(err.toString());
      })

    this.msghtm = "";
    this.chathisto = "";
  }

  SendMsg(msgp: string) {
    debugger;
    this.element = document.getElementById('messagesList') as HTMLElement;
    if (this.element == null)
      this.msghtm = ""
    else
      this.msghtm = this.element.innerHTML;

    this.element = document.getElementById('chathistory') as HTMLElement;
    if (this.element == null)
      this.chathisto = ""
    else
      this.chathisto = this.element.innerHTML;

    this._hubConnection.invoke("PassMessage", this.currentuser.id, this.respectiveuser.id, this.msg, this.chathisto + this.msghtm, this.currentuser.nickName, this.respectiveuser.nickName)
      .then(function () { })
      .catch(function (err) {
        return console.error(err.toString());
      })
  }
}
