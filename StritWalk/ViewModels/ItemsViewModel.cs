using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Com.OneSignal;
using Plugin.AudioRecorder;
using Plugin.MediaManager;
using System.Windows.Input;

namespace StritWalk
{
    public class ItemsViewModel : BaseViewModel
    {
        // variables
        public int start;
        string result = string.Empty;
        public ObservableRangeCollection<Item> Items { get; set; }
        //public ObservableCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public Command PostCommand { get; }
        public Command RecCommand { get; }
        public Command PlayCommand { get; }
        public CustomEditor PostEditor { get; set; }
        public Command ILikeThis { get; }
        public User me;
        public ICommand ICommentThis;
        //AudioRecorderService recorder;
        IAudioPlayer player;
        TestService testService;
        //string filePath;
        string audioName = String.Empty;
        public Command IPlayThis { get; }
        string audioFilePath = string.Empty;

        // properties
        string newPostDescription = string.Empty;
        public string NewPostDescription
        {
            get { return newPostDescription; }
            set { SetProperty(ref newPostDescription, value); }
        }
        string endText = string.Empty;
        public string EndText
        {
            get { return endText; }
            set { SetProperty(ref endText, value); }
        }
        Color postPlaceholder = (Color)Application.Current.Resources["Testo3"];
        public Color PostPlaceholder
        {
            get { return postPlaceholder; }
            set { SetProperty(ref postPlaceholder, value); }
        }
        bool isNotEnd = false;
        public bool IsNotEnd { get { return isNotEnd; } set { SetProperty(ref isNotEnd, value); } }
        string recButton = "Rec";
        public string RecButton { get { return recButton; } set { SetProperty(ref recButton, value); } }
        bool isAudioPost = false;
        public bool IsAudioPost { get { return isAudioPost; } set { SetProperty(ref isAudioPost, value); } }


        // CONSTRUCTOR
        public ItemsViewModel()
        {
            Title = "StritWalk";
            //Title = "Seahorse";
            Items = new ObservableRangeCollection<Item>();
            //Items = new ObservableCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            PostCommand = new Command(async (par1) => await PostTask((object)par1));
            RecCommand = new Command((par1) => RecTask((object)par1));
            PlayCommand = new Command((par1) => PlayTask((object)par1));
            ILikeThis = new Command(async (par1) => await ILikeThisTask((object)par1));
            ICommentThis = new Command(async (par1) => await ICommentThisTask((object)par1));
            IPlayThis = new Command(async (par1) => await IPlayThisTask((object)par1));
            me = new User();
            //Task.Run(() => GetMyUser());

            //MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            //{
            //    var _item = item as Item;
            //    Items.Add(_item);
            //    await DataStore.AddItemAsync(_item);
            //});

            MessagingCenter.Subscribe<CloudDataStore, bool>(this, "NotEnd", (sender, arg) =>
            {
                IsNotEnd = arg;
            });

            MessagingCenter.Subscribe<App, string>(this, "OnResume", (sender, arg) =>
            {
                LoadItemsCommand.Execute(null);
            });


            //MessagingCenter.Subscribe<ItemDetailViewModel, bool>(this, "NotEnd", (sender, arg) =>
            //{
            //    IsNotEnd = arg;
            //});

            //start with notifications            
            //OneSignal.Current.GetTags(getNotifTags);

            //dev10n
            //audio record service
            //recorder = new AudioRecorderService
            //{
            //    StopRecordingAfterTimeout = true,
            //    StopRecordingOnSilence = true,
            //    TotalAudioTimeout = TimeSpan.FromSeconds(10),
            //    AudioSilenceTimeout = TimeSpan.FromSeconds(2)
            //};

            //recorder.AudioInputReceived += (object sender, string file) =>
            //{
            //    RecButton = "Rec";
            //    filePath = file;
            //    IsAudioPost = true;
            //};

            //player
            player = DependencyService.Get<IAudioPlayer>();
            player.FinishedPlaying += Player_FinishedPlaying;
            player.InitRecord();
            player.FinishedRecording += Player_FinishedRecording;

            //test service
            testService = new TestService();
            testService.test();

            //stop external player
            CrossMediaManager.Current.MediaQueue.Repeat = Plugin.MediaManager.Abstractions.Enums.RepeatType.None;
            CrossMediaManager.Current.MediaFinished += (sender, e) =>
            {
                Debug.WriteLine("finished playing external file");
                CrossMediaManager.Current.Stop();
                CrossMediaManager.Current.MediaQueue.Clear();
            };

        }

        void insertItem(Item item)
        {
            try
            {
                Items.Insert(0, item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(" EX " + ex);
            }

        }

        async Task PostTask(object par1)
        {
            if (!IsPosting) return;
            try
            {
                //start loading phase
                IsLoading = true;

                //upload del file audio
                if (isAudioPost) audioName = await TryUploadAudio();
                // Post method
                result = await TryPostAsync();
            }
            finally
            {
                IsLoading = false;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    IsAudioPost = false;
                    MessagingCenter.Send(this, "IsAudioPost", IsAudioPost);

                    audioFilePath = "";
                    var newitem = new Item
                    {
                        Id = result,
                        Nuovo = true,
                        Creator = Settings.UserId,
                        Description = newPostDescription,
                        Likes = "0",
                        Comments_count = "0",
                        Distanza = "0",
                        Liked_me = "0",
                        Liked_me_color = (Color)Application.Current.Resources["Testo4"],
                        Comments = new Newtonsoft.Json.Linq.JArray(),
                        Duedate = null,
                        Audio = audioName
                    };
                    //Items.Insert(0, newitem);
                    insertItem(newitem);

                    Settings.Num_posts += 1;
                    me.Num_posts += 1;
                    PostsN = new FormattedString
                    {
                        Spans =
                        {
                            new Span { Text = "Posts" + "\n", FontSize=11.0F, ForegroundColor=(Color)Application.Current.Resources["Testo1"]},
                            new Span { Text = me.Num_posts.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=(Color)Application.Current.Resources["Testo2"] }
                        }
                    };

                    string text = "Posted. Do you want to post something else?";
                    PostEditor.Placeholder = text;

                    if (Device.iOS == Device.RuntimePlatform)
                    {
                        PostEditor.Text = text;
                    }
                    else
                    {
                        PostEditor.Text = "";
                    }
                }
                else
                {
                    string text = "Do you want to post something?";
                    PostEditor.Placeholder = text;
                }
                IsPosting = false;
                MessagingCenter.Send(this, "IsPosting", IsPosting);
                PostEditor.Unfocus();
            }
        }

        void RecTask(object par1)
        {
            //play audio sample code
            //await CrossMediaManager.Current.Play("http://www.sample-videos.com/audio/mp3/crowd-cheering.mp3");

            try
            {
                if (RecButton == "Rec")
                {
                    RecButton = "Stop";
                    audioFilePath = player.StartRecording();
                }
                else
                {
                    RecButton = "Rec";
                    player.StopRecording();
                }
                //if (!recorder.IsRecording) //Record button clicked
                //{
                //    recorder.StopRecordingOnSilence = false;
                //    RecButton = "Stop";
                //    //start recording audio
                //    player.SolveErrors();
                //    var audioRecordTask = await recorder.StartRecording();

                //    await audioRecordTask;

                //}
                //else //Stop button clicked
                //{
                //    //stop the recording
                //    await recorder.StopRecording();
                //    RecButton = "Rec";
                //}


            }
            catch (Exception ex)
            {
                //blow up the app!
                //throw ex;
                Debug.WriteLine(ex);
            }
        }

        void PlayTask(object par1)
        {
            if (!IsAudioPost)
            {
                return;
            }
            try
            {
                //filePath = recorder.GetAudioFilePath();
                //Console.WriteLine("@@@@@ play " + audioFilePath);
                if (audioFilePath != null)
                {
                    //await CrossMediaManager.Current.Play(filePath);
                    player.Play(audioFilePath);
                }
            }
            catch (Exception ex)
            {
                //blow up the app!
                //throw ex;
                Console.WriteLine("@@@ exception : " + ex);
            }
        }

        public async Task<string> TryPostAsync()
        {
            return await DataStore.Post("", "", audioName, "", "", newPostDescription);
        }

        //ricarica la pagina
        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;
            await Task.Run(() => GetMyUser());
            EndText = "";
            start = 0;
            IsNotEnd = false;
            Settings.listEnd = false;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                Items.ReplaceRange(items);
                //foreach (var item in items)
                //{
                //    Items.Add(item);
                //}
                IsNotEnd = true;
                EndText = "";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                //IsNotEnd = false;
                IsBusy = false;
            }
        }

        //mettere o togliere un like
        async Task ILikeThisTask(object par1)
        {
            Console.WriteLine("@@@@@ I LIKE THIS TASK");
            if (IsWorking)
                return;

            IsWorking = true;
            Item item = par1 as Item;
            string action = "addLikePost";
            if (item.Liked_me == "1")
            {
                action = "removeLikePost";
                var num = Int32.Parse(item.LikesNum);
                num -= 1;
                item.Likes = num.ToString();
                item.Liked_me = "0";
                item.NumberOfLikes = num.ToString();
                item.Liked_me_color = (Color)Application.Current.Resources["Testo4"];
            }
            else
            {
                var num = Int32.Parse(item.LikesNum);
                num += 1;
                item.Likes = num.ToString();
                item.Liked_me = "1";
                item.NumberOfLikes = num.ToString();
                item.Liked_me_color = (Color)Application.Current.Resources["App2"];
            }

            var res = await DataStore.ILikeThis((string)item.Id, action);

            if (res == 2)
            {

            }
            else if (res == 0)
            {

            }

            IsWorking = false;
        }

        //ottieni il mio utente
        async Task GetMyUser()
        {
            me = await DataStore.GetMyUser(me);
            PostsN = new FormattedString
            {
                Spans =
                    {
                    new Span { Text = "Posts" + "\n", FontSize=11.0F, ForegroundColor=(Color)Application.Current.Resources["Testo1"]},
                    new Span { Text = me.Num_posts.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=(Color)Application.Current.Resources["Testo2"] }
                    }
            };
            LikesN = new FormattedString
            {
                Spans =
                    {
                    new Span { Text = "Liked" + "\n", FontSize=11.0F, ForegroundColor=(Color)Application.Current.Resources["Testo1"]},
                    new Span { Text = me.Num_likes.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=(Color)Application.Current.Resources["Testo2"] }
                    }
            };
            FriendsN = new FormattedString
            {
                Spans =
                    {
                    new Span { Text = "Followers" + "\n", FontSize=11.0F, ForegroundColor=(Color)Application.Current.Resources["Testo1"]},
                    new Span { Text = me.Num_friends.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=(Color)Application.Current.Resources["Testo2"] }
                    }
            };
        }

        async Task ICommentThisTask(object par1)
        {
            CustomTabbedPage page = Application.Current.MainPage as CustomTabbedPage;
            page.TabBarHidden = true;
            var newPage = new ItemDetailPage(new ItemDetailViewModel(par1));
            //NavigationPage.SetHasNavigationBar(newPage, false);
            //await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(par1)));
            await Navigation.PushAsync(newPage);
            //await Navigation.PushModalAsync(new NavigationPage(new ItemDetailPage(new ItemDetailViewModel(par1))));
            //await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new ItemDetailPage(new ItemDetailViewModel(par1))));

            //cambiamento pagina principale
            //Application.Current.MainPage = new NavigationPage(new ItemDetailPage(new ItemDetailViewModel(par1)))
            //{
                
            //};
        }

        //notifications setup OBSOLETE
        void getNotifTags(Dictionary<string, object> tags)
        {

            try
            {

                if (tags == null || tags.Count == 0)
                {

                    OneSignal.Current.SetSubscription(true);
                    OneSignal.Current.SendTags(new Dictionary<string, string>()
                    {
                        {"UserId", Settings.AuthToken }, //purtroppo il nome della variabile è rimasto quello da inizio sviluppo... :( 
                        {"UserName", Settings.UserId }
                    });
                    OneSignal.Current.IdsAvailable(getNotifId);
                    return;
                }
                foreach (var tag in tags)
                    Console.WriteLine("@@@@@@@@@@@ tags : " + tag.Key + ":" + tag.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine("@@@@@@@@@@@ onesignal exception " + ex);
            }
        }

        //OBSOLETE
        private void getNotifId(string userID, string pushToken)
        {
            //salvare notification id nel server

            Settings.Notification_id = userID;
            DataStore.addPushId(userID);
        }

        //finito di riprodurre musica
        void Player_FinishedPlaying(object sender, EventArgs e)
        {
            Debug.WriteLine("finished playing");
        }


        //upload audio task launch
        public async Task<string> TryUploadAudio()
        {
            return await DataStore.UploadAudio(audioFilePath);
        }


        //play audio task
        async Task IPlayThisTask(object par1)
        {
            Item item = par1 as Item;
            //await CrossMediaManager.Current.Stop();
            CrossMediaManager.Current.MediaQueue.Clear();
            await CrossMediaManager.Current.Stop();
            await CrossMediaManager.Current.Play("https://www.hackweb.it/api/uploads/audio/" + item.Audio);
        }

        void Player_FinishedRecording(object sender, EventArgs e)
        {
            Console.WriteLine("@@@@@ finishedRecording " + audioFilePath);
            RecButton = "Rec";
            IsAudioPost = true;
            MessagingCenter.Send(this, "IsAudioPost", IsAudioPost);
        }
    }
}
