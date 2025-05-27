$(function () {

  $('.whl-video').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();

    var i = $(e.currentTarget).data('videoid');
    var t = $(e.currentTarget).data('videotitle');

    if (i !== undefined && i !== null && i.trim().length > 0
          && t !== undefined && t !== null && t.trim().length > 0) {
      $('.whl-video-title').html(t);
      $('.whl-video-iframe').attr('src', 'https://www.youtube.com/embed/' + i);
    }
  });

});