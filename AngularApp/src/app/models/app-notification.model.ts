export interface AppNotification {
  type: string;
  payload: {
    message: string;
  };
  receivedAt: Date;
}