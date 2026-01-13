import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {
  newsletterName = '';
  newsletterEmail = '';

  onSubmitNewsletter(event: Event): void {
    event.preventDefault();
    if (this.newsletterName && this.newsletterEmail) {
      this.newsletterName = '';
      this.newsletterEmail = '';
    }
  }
}