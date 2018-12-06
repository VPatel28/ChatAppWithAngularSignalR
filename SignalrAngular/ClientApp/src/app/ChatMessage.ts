import { Injectable } from "@angular/core";

@Injectable()
export class ChatMessage {
  id: number;
  userName: string;
  nickName: string;
  chatHistory: string;
  grpName: string;
  connectionId: string;
  isLoggedIn: number;
}
