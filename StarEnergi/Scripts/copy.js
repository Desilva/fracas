function copyToClip(str) {
    var copyFrom = $('<textarea/>');
    copyFrom.text(str);
    $('#page').append(copyFrom);
    copyFrom.focus();
    copyFrom.select();
    document.execCommand('copy');
}