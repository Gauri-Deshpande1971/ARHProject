import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationServiceService {  private notifications = new BehaviorSubject<string[]>([]);
  currentNotifications = this.notifications.asObservable();

  constructor() { }

  addNotification(message: string) {
    let current = this.notifications.value;
    current.push(message);
    this.notifications.next(current);
  }

  clearNotifications() {
    this.notifications.next([]);
  }
}
