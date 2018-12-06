import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ChatMessage } from './ChatMessage';
import { catchError } from 'rxjs/operators';


const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type': 'application/json',
    'Authorization': 'my-auth-token'
  })
};

@Injectable({
  providedIn: 'root'
})

export class ChatService {
  myAppUrl = 'api/ChatMessages/';
  constructor(private http: HttpClient) { }

  getConnectedUser() {
    return this.http.get<ChatMessage[]>(this.myAppUrl + '/GetChatMessage')
      .pipe(
        catchError(e => throwError(e))
      );
  }

  UpdateUserDetails(user: ChatMessage): Observable<ChatMessage> {
    return this.http.put<ChatMessage>(this.myAppUrl + '/RemoveLoggedIn/' + user.id, JSON.stringify(user), httpOptions)
      .pipe(
        catchError(e => throwError(e))
      );
  }

  AddUserDetails(user: ChatMessage): Observable<ChatMessage> {
    return this.http.post<ChatMessage>(this.myAppUrl + '/PostChatMessage', JSON.stringify(user), httpOptions)
      .pipe(
        catchError(e => throwError(e))
      );
  }
}
