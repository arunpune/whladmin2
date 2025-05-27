$(function () {

  $('.whl-action-add-video').on('click', function (e) {
    e.preventDefault();
    $.get(videoConfigAddUrl)
      .done(function (response) {
        $('#whl-title-video-action').html('Add Video');
        $('#hidVideoAction').val('ADD');
        $('#videoEditorModal').find('div.modal-body').html(response);
        $('#videoEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-edit-video').on('click', function (e) {
    e.preventDefault();
    var videoId = $(e.currentTarget).data('videoid');
    $.get(videoConfigEditUrl + '?videoId=' + videoId)
      .done(function (response) {
        $('#whl-title-video-action').html('Edit Video');
        $('#hidVideoAction').val('EDIT');
        $('#videoEditorModal').find('div.modal-body').html(response);
        $('#videoEditorModal').modal('show');
        $('#lblSaveErrorMessage').html('');
        $('#divSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-video').on('click', function (e) {
    e.preventDefault();
    $('#lblSaveErrorMessage').html('');
    $('#divSaveErrorMessage').hide();
    var a = $('#hidVideoAction').val();
    var videoTitle = $('#Title').val();
    var url = a === 'EDIT' ? videoConfigEditUrl : videoConfigAddUrl;
    var data = {
      VideoId: $('#VideoId').val(),
      Title: $('#Title').val(),
      Text: $('#Text').val(),
      Url: $('#Url').val(),
      DisplayOrder: $('#DisplayOrder').val(),
      DisplayOnHomePageInd: $('#DisplayOnHomePageInd').prop('checked'),
      Active: $('#Active').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#videoEditorModal').find('div.modal-body').html('');
        $('#videoEditorModal').modal('hide');
        fnShowToast('SUCCESS', 'Video ' + videoTitle + ' saved, refreshing page.');
      })
      .fail(function (e) {
        var message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save video information';
        $('#lblSaveErrorMessage').html(e.responseJSON.message);
        $('#divSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-video').on('click', function (e) {
    e.preventDefault();
    var videoId = $(e.currentTarget).data('videoid');
    var videoTitle = $(e.currentTarget).data('videotitle');
    if (confirm('Are you sure you want to delete video - ' + videoTitle + '?')) {
      $.post(videoConfigDeleteUrl + '?videoId=' + videoId)
        .done(function (response) {
          fnShowToast('SUCCESS', 'Video ' + videoTitle + ' deleted, refreshing page.');
        });
    }
  });

});