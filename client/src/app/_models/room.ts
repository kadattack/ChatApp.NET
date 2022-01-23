export interface Room {
  id: number
  roomName: string,
  roomTopic: string,
  hostName: string,
  description: string,
  avatarUrl: string,
  participants: [
    {
      userName: string,
      avatarUrl: string
    }
  ],
  messages: [
    {
      userCreated: string,
      messageIsInRoom: string,
      body: string,
      created: string,
      updated: string,
      avatarUrl: string
    }
  ],
  updated: string
}
