import { Component, input, output, signal, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductImage } from '../../../core/models/product.model';
import { ProductService } from '../../../core/services/product.service';

@Component({
  selector: 'app-image-viewer',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './image-viewer.html',
  styleUrls: ['./image-viewer.css']
})
export class ImageViewerComponent implements OnInit, OnDestroy {

  readonly productService = inject(ProductService);
  images = input.required<ProductImage[]>();
  activeIndex = input<number>(0);
  closeModal = output<void>();

  currentIndex = signal<number>(0);
  segmentProgress = signal<number>(0);      // Active story segment fill (0-100%)
  isHovered = signal(false);                 // Track mouse hover to hold/pause

  private segmentInterval: any = null;
  private timerId = 0;                       // Token to kill stale intervals instantly

  private readonly displayDurationMs = 3600; // 3.6 seconds per image
  private readonly tickMs = 37;              // Smoothness step interval

  ngOnInit() {
    const currentImages = this.images();
    const initialIdx = this.activeIndex() || 0;

    if (currentImages && currentImages.length > 0) {
      const safeIdx = Math.min(Math.max(0, initialIdx), currentImages.length - 1);
      this.currentIndex.set(safeIdx);
      this.startTimer();
    }
  }

  startTimer() {
    this.clearTimer();

    this.timerId++;
    const currentTimerId = this.timerId;

    this.segmentProgress.set(0);

    const increment = (this.tickMs / this.displayDurationMs) * 100;

    this.segmentInterval = setInterval(() => {
      if (this.timerId !== currentTimerId) return;
      if (this.isHovered()) return; // Pause on hover

      // Calculate the next progress value safely outside the signal update
      const currentVal = this.segmentProgress();
      const nextVal = currentVal + increment;

      if (nextVal >= 100) {
        this.segmentProgress.set(100);
        this.clearTimer();
        this.nextImage(); // Safely called once outside the update block
      } else {
        this.segmentProgress.set(nextVal);
      }
    }, this.tickMs);
  }

  nextImage() {
    const total = this.images().length;
    const nextIdx = (this.currentIndex() + 1) % total;
    this.currentIndex.set(nextIdx);
    this.startTimer();
  }

  prevImage() {
    const total = this.images().length;
    const prevIdx = (this.currentIndex() - 1 + total) % total;
    this.currentIndex.set(prevIdx);
    this.startTimer();
  }

  goToImage(index: number) {
    if (index === this.currentIndex()) return;
    this.currentIndex.set(index);
    this.startTimer();
  }

  onHoverStart() {
    this.isHovered.set(true);
  }

  onHoverEnd() {
    this.isHovered.set(false);
  }

  getSegmentFill(index: number): number {
    const current = this.currentIndex();
    if (index < current) return 100;
    if (index > current) return 0;
    return this.segmentProgress();
  }

  onClose() {
    this.clearTimer();
    this.closeModal.emit();
  }

  private clearTimer() {
    if (this.segmentInterval) {
      clearInterval(this.segmentInterval);
      this.segmentInterval = null;
    }
  }

  resolveImage(url?: string | null): string {
    return this.productService.resolveMediaUrl(url);
  }

  ngOnDestroy() {
    this.clearTimer();
  }
}