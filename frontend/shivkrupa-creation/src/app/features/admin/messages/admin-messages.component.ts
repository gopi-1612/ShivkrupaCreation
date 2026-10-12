import { Component, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { ContactService } from '../../../core/services/contact.service';
import { ContactMessage } from '../../../core/models/contact.model';

@Component({
  selector: 'app-admin-messages',
  standalone: true,
  imports: [CommonModule, DatePipe, ConfirmDialogComponent],
  templateUrl: './admin-messages.component.html',
  styleUrl: './admin-messages.component.scss'
})
export class AdminMessagesComponent implements OnInit {
  messages: ContactMessage[] = [];
  loading = true;
  selected: ContactMessage | null = null;
  deleteTarget: ContactMessage | null = null;

  constructor(private contactService: ContactService) { }

  ngOnInit(): void {
    this.fetchMessages();
  }

  fetchMessages(): void {
    this.loading = true;
    this.contactService.getMessages().subscribe({
      next: messages => { this.messages = messages; this.loading = false; },
      error: () => (this.loading = false)
    });
  }

  openMessage(message: ContactMessage): void {
    this.selected = message;
    if (!message.isRead) {
      this.contactService.markRead(message.id).subscribe(updated => {
        message.isRead = updated.isRead;
      });
    }
  }

  closeMessage(): void {
    this.selected = null;
  }

  confirmDelete(message: ContactMessage, event: Event): void {
    event.stopPropagation();
    this.deleteTarget = message;
  }

  cancelDelete(): void {
    this.deleteTarget = null;
  }

  performDelete(): void {
    if (!this.deleteTarget) return;
    const id = this.deleteTarget.id;
    this.contactService.deleteMessage(id).subscribe(() => {
      this.messages = this.messages.filter(m => m.id !== id);
      if (this.selected?.id === id) this.selected = null;
      this.deleteTarget = null;
    });
  }

  get unreadCount(): number {
    return this.messages.filter(m => !m.isRead).length;
  }
}
